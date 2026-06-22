namespace QuotationAccelerator.Inbox.Application.SendProposalReply;

using FluentValidation;
using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Results;

public sealed record SendProposalReplyCommand(
    string InboxMessageId,
    string Subject,
    string Body,
    string? AttachmentFilePath) : ICommand<Result>;

public sealed class SendProposalReplyValidator : AbstractValidator<SendProposalReplyCommand>
{
    public SendProposalReplyValidator()
    {
        RuleFor(command => command.InboxMessageId).NotEmpty();
        RuleFor(command => command.Subject).NotEmpty();
        RuleFor(command => command.Body).NotEmpty();
    }
}

public sealed class SendProposalReplyHandler(
    IMailClient mailClient,
    IInboxMessageRepository messageRepository,
    IValidator<SendProposalReplyCommand> validator)
    : ICommandHandler<SendProposalReplyCommand, Result>
{
    public async Task<Result> HandleAsync(SendProposalReplyCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure(validation.Errors.Select(error => error.ErrorMessage));
        }

        if (!mailClient.IsConnected)
        {
            return Result.Failure("Mail account is not connected.");
        }

        var message = await messageRepository.GetByIdAsync(command.InboxMessageId, cancellationToken);
        if (message is null)
        {
            return Result.Failure("Inbox message was not found.");
        }

        IReadOnlyList<MailAttachmentPayload>? attachments = null;
        if (!string.IsNullOrWhiteSpace(command.AttachmentFilePath) && File.Exists(command.AttachmentFilePath))
        {
            var content = await File.ReadAllBytesAsync(command.AttachmentFilePath, cancellationToken);
            var contentType = command.AttachmentFilePath.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)
                ? "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                : "application/pdf";

            attachments =
            [
                new MailAttachmentPayload
                {
                    FileName = Path.GetFileName(command.AttachmentFilePath),
                    Content = content,
                    ContentType = contentType,
                },
            ];
        }

        var sendResult = await mailClient.SendReplyAsync(
            message.GraphMessageId,
            message.FromAddress,
            command.Subject,
            command.Body,
            attachments,
            cancellationToken);

        if (sendResult.IsFailure)
        {
            return sendResult;
        }

        await messageRepository.UpdateStatusAsync(
            message.Id,
            InboxMessageStatus.Replied,
            cancellationToken);

        return Result.Success();
    }
}
