namespace QuotationAccelerator.Inbox.Application.EscalateToSupport;

using FluentValidation;
using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Results;

public sealed record EscalateToSupportCommand(string InboxMessageId, string? Notes) : ICommand<Result>;

public sealed class EscalateToSupportValidator : AbstractValidator<EscalateToSupportCommand>
{
    public EscalateToSupportValidator()
    {
        RuleFor(command => command.InboxMessageId).NotEmpty();
    }
}

public sealed class EscalateToSupportHandler(
    IInboxMessageRepository messageRepository,
    ISupportTicketRepository ticketRepository,
    IValidator<EscalateToSupportCommand> validator)
    : ICommandHandler<EscalateToSupportCommand, Result>
{
    public async Task<Result> HandleAsync(EscalateToSupportCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure(validation.Errors.Select(error => error.ErrorMessage));
        }

        var message = await messageRepository.GetByIdAsync(command.InboxMessageId, cancellationToken);
        if (message is null)
        {
            return Result.Failure("Inbox message was not found.");
        }

        var now = DateTimeOffset.UtcNow;
        await ticketRepository.UpsertAsync(new SupportTicket
        {
            Id = Guid.NewGuid().ToString("N"),
            InboxMessageId = message.Id,
            Subject = message.Subject,
            FromAddress = message.FromAddress,
            Status = SupportTicketStatus.Open,
            Notes = command.Notes,
            CreatedAt = now,
            UpdatedAt = now,
        }, cancellationToken);

        await messageRepository.UpdateStatusAsync(
            message.Id,
            InboxMessageStatus.Escalated,
            cancellationToken);

        return Result.Success();
    }
}
