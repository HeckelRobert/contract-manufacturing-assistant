namespace QuotationAccelerator.Infrastructure.Mail;

using QuotationAccelerator.Catalog.Application.Abstractions;
using QuotationAccelerator.Inbox.Application.Abstractions;

public sealed class MailAttachmentStore(IAppPathProvider appPathProvider) : IMailAttachmentStore
{
    public async Task<string> SaveAttachmentAsync(
        string messageId,
        string fileName,
        byte[] content,
        CancellationToken cancellationToken)
    {
        var directory = Path.Combine(appPathProvider.GetApplicationDirectory(), "mail-attachments", messageId);
        Directory.CreateDirectory(directory);

        var safeFileName = Path.GetFileName(fileName);
        var targetPath = Path.Combine(directory, safeFileName);
        await File.WriteAllBytesAsync(targetPath, content, cancellationToken);
        return targetPath;
    }

    public Task ClearAllAsync(CancellationToken cancellationToken)
    {
        var directory = Path.Combine(appPathProvider.GetApplicationDirectory(), "mail-attachments");
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, recursive: true);
        }

        return Task.CompletedTask;
    }
}
