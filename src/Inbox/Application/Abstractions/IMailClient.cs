namespace QuotationAccelerator.Inbox.Application.Abstractions;

using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.SharedKernel.Results;

public interface IMailClient
{
    bool IsConnected { get; }

    Task<Result> ConnectInteractiveAsync(CancellationToken cancellationToken);

    Task DisconnectAsync(CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<RemoteMailMessage>>> FetchMessagesAsync(
        DateTimeOffset? since,
        CancellationToken cancellationToken);

    Task<Result<byte[]>> DownloadAttachmentAsync(
        string graphMessageId,
        string attachmentId,
        CancellationToken cancellationToken);

    Task<Result> SendReplyAsync(
        string graphMessageId,
        string toAddress,
        string subject,
        string body,
        IReadOnlyList<MailAttachmentPayload>? attachments,
        CancellationToken cancellationToken);
}

public sealed class RemoteMailMessage
{
    public required string GraphMessageId { get; init; }

    public required string Subject { get; init; }

    public required string FromAddress { get; init; }

    public string? FromDisplayName { get; init; }

    public required DateTimeOffset ReceivedAt { get; init; }

    public string? BodyPreview { get; init; }

    public string? BodyText { get; init; }

    public IReadOnlyList<RemoteMailAttachment> Attachments { get; init; } = [];
}

public sealed class RemoteMailAttachment
{
    public required string Id { get; init; }

    public required string FileName { get; init; }

    public string? ContentType { get; init; }
}

public sealed class MailAttachmentPayload
{
    public required string FileName { get; init; }

    public required byte[] Content { get; init; }

    public string ContentType { get; init; } = "application/octet-stream";
}
