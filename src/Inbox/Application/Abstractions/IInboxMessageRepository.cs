namespace QuotationAccelerator.Inbox.Application.Abstractions;

using QuotationAccelerator.Inbox.Domain;

public interface IInboxMessageRepository
{
    Task<IReadOnlyList<InboxMessage>> GetAllAsync(CancellationToken cancellationToken);

    Task<InboxMessage?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<InboxMessage?> GetByGraphMessageIdAsync(string graphMessageId, CancellationToken cancellationToken);

    Task UpsertAsync(InboxMessage message, CancellationToken cancellationToken);

    Task UpdateCategoryAsync(
        string id,
        InboxMessageCategory category,
        string? suggestedReplyBody,
        CancellationToken cancellationToken);

    Task UpdateStatusAsync(string id, InboxMessageStatus status, CancellationToken cancellationToken);

    Task UpdateAttachmentLocalPathAsync(
        string messageId,
        string attachmentId,
        string localPath,
        CancellationToken cancellationToken);

    Task<int> ClearAllAsync(CancellationToken cancellationToken);
}
