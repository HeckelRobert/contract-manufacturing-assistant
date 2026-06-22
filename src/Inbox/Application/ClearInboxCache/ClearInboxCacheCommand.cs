namespace QuotationAccelerator.Inbox.Application.ClearInboxCache;

using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Results;

public sealed record ClearInboxCacheCommand : ICommand<Result<int>>;

public sealed class ClearInboxCacheHandler(
    IInboxMessageRepository messageRepository,
    ISupportTicketRepository supportTicketRepository,
    IMailAccountRepository accountRepository,
    IMailAttachmentStore attachmentStore)
    : ICommandHandler<ClearInboxCacheCommand, Result<int>>
{
    public async Task<Result<int>> HandleAsync(ClearInboxCacheCommand command, CancellationToken cancellationToken)
    {
        var deletedMessages = await messageRepository.ClearAllAsync(cancellationToken);
        await supportTicketRepository.ClearAllAsync(cancellationToken);
        await attachmentStore.ClearAllAsync(cancellationToken);
        await accountRepository.ClearLastFetchedAtAsync(cancellationToken);
        return Result<int>.Success(deletedMessages);
    }
}
