namespace QuotationAccelerator.Inbox.Application.Abstractions;

public interface IMailAttachmentStore
{
    Task<string> SaveAttachmentAsync(
        string messageId,
        string fileName,
        byte[] content,
        CancellationToken cancellationToken);

    Task ClearAllAsync(CancellationToken cancellationToken);
}
