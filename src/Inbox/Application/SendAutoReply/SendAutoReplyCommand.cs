namespace QuotationAccelerator.Inbox.Application.SendAutoReply;

using FluentValidation;
using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Results;

public sealed record SendAutoReplyCommand(string InboxMessageId, string ReplyBody) : ICommand<Result>;

public sealed class SendAutoReplyValidator : AbstractValidator<SendAutoReplyCommand>
{
    public SendAutoReplyValidator()
    {
        RuleFor(command => command.InboxMessageId).NotEmpty();
        RuleFor(command => command.ReplyBody).NotEmpty();
    }
}

public sealed class SendAutoReplyHandler(
    IMailClient mailClient,
    IInboxMessageRepository messageRepository,
    IValidator<SendAutoReplyCommand> validator)
    : ICommandHandler<SendAutoReplyCommand, Result>
{
    public async Task<Result> HandleAsync(SendAutoReplyCommand command, CancellationToken cancellationToken)
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

        if (message.Category != InboxMessageCategory.AutoAnswerable)
        {
            return Result.Failure("Only auto-answerable messages can be replied to with default mail responses.");
        }

        var sendResult = await mailClient.SendReplyAsync(
            message.GraphMessageId,
            message.FromAddress,
            $"Re: {message.Subject}",
            command.ReplyBody,
            attachments: null,
            cancellationToken);

        if (sendResult.IsFailure)
        {
            return sendResult;
        }

        await messageRepository.UpdateStatusAsync(
            message.Id,
            InboxMessageStatus.AutoReplied,
            cancellationToken);

        return Result.Success();
    }
}
