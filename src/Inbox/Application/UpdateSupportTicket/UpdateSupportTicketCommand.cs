namespace QuotationAccelerator.Inbox.Application.UpdateSupportTicket;

using FluentValidation;
using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Results;

public sealed record UpdateSupportTicketCommand(
    string TicketId,
    SupportTicketStatus Status,
    string? Notes) : ICommand<Result>;

public sealed class UpdateSupportTicketValidator : AbstractValidator<UpdateSupportTicketCommand>
{
    public UpdateSupportTicketValidator()
    {
        RuleFor(command => command.TicketId).NotEmpty();
    }
}

public sealed class UpdateSupportTicketHandler(
    ISupportTicketRepository ticketRepository,
    IValidator<UpdateSupportTicketCommand> validator)
    : ICommandHandler<UpdateSupportTicketCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateSupportTicketCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure(validation.Errors.Select(error => error.ErrorMessage));
        }

        var tickets = await ticketRepository.GetAllAsync(cancellationToken);
        var ticket = tickets.FirstOrDefault(item => item.Id == command.TicketId);
        if (ticket is null)
        {
            return Result.Failure("Support ticket was not found.");
        }

        await ticketRepository.UpsertAsync(new SupportTicket
        {
            Id = ticket.Id,
            InboxMessageId = ticket.InboxMessageId,
            Subject = ticket.Subject,
            FromAddress = ticket.FromAddress,
            Status = command.Status,
            Notes = command.Notes ?? ticket.Notes,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = DateTimeOffset.UtcNow,
        }, cancellationToken);

        return Result.Success();
    }
}
