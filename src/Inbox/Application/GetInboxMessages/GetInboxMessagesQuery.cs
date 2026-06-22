namespace QuotationAccelerator.Inbox.Application.GetInboxMessages;

using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.SharedKernel.Abstractions;

public sealed record GetInboxMessagesQuery : IQuery<IReadOnlyList<InboxMessage>>;

public sealed class GetInboxMessagesHandler(IInboxMessageRepository repository)
    : IQueryHandler<GetInboxMessagesQuery, IReadOnlyList<InboxMessage>>
{
    public Task<IReadOnlyList<InboxMessage>> HandleAsync(
        GetInboxMessagesQuery query,
        CancellationToken cancellationToken) =>
        repository.GetAllAsync(cancellationToken);
}
