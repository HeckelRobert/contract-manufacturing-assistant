namespace QuotationAccelerator.Infrastructure.Persistence;

public sealed class InboxAttachmentEntity
{
    public required string Id { get; set; }

    public required string InboxMessageId { get; set; }

    public required string FileName { get; set; }

    public string? ContentType { get; set; }

    public string? LocalPath { get; set; }

    public InboxMessageEntity? Message { get; set; }
}
