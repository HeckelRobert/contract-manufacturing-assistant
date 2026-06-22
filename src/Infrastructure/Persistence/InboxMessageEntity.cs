namespace QuotationAccelerator.Infrastructure.Persistence;

public sealed class InboxMessageEntity
{
    public required string Id { get; set; }

    public required string GraphMessageId { get; set; }

    public required string Subject { get; set; }

    public required string FromAddress { get; set; }

    public string? FromDisplayName { get; set; }

    public DateTimeOffset ReceivedAt { get; set; }

    public string? BodyPreview { get; set; }

    public string? BodyText { get; set; }

    public int Category { get; set; }

    public int Status { get; set; }

    public string? SuggestedReplyBody { get; set; }

    public ICollection<InboxAttachmentEntity> Attachments { get; set; } = [];
}
