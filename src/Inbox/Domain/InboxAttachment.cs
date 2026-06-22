namespace QuotationAccelerator.Inbox.Domain;

public sealed class InboxAttachment
{
    public required string Id { get; init; }

    public required string FileName { get; init; }

    public string? ContentType { get; init; }

    public string? LocalPath { get; init; }

    public bool IsPdf =>
        FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
        || string.Equals(ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase);
}
