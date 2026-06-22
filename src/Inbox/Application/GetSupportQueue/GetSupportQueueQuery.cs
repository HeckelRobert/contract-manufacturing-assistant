namespace QuotationAccelerator.Inbox.Application.GetSupportQueue;

using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.SharedKernel.Abstractions;

public sealed record GetSupportQueueQuery : IQuery<IReadOnlyList<SupportTicket>>;

public sealed class GetSupportQueueHandler(ISupportTicketRepository repository)
    : IQueryHandler<GetSupportQueueQuery, IReadOnlyList<SupportTicket>>
{
    public Task<IReadOnlyList<SupportTicket>> HandleAsync(
        GetSupportQueueQuery query,
        CancellationToken cancellationToken) =>
        repository.GetAllAsync(cancellationToken);
}
