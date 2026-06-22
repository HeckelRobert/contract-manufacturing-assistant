namespace QuotationAccelerator.Inbox.Application.Abstractions;

using QuotationAccelerator.Inbox.Domain;

public interface ISupportTicketRepository
{
    Task<IReadOnlyList<SupportTicket>> GetOpenAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<SupportTicket>> GetAllAsync(CancellationToken cancellationToken);

    Task<SupportTicket?> GetByInboxMessageIdAsync(string inboxMessageId, CancellationToken cancellationToken);

    Task UpsertAsync(SupportTicket ticket, CancellationToken cancellationToken);

    Task ClearAllAsync(CancellationToken cancellationToken);
}
