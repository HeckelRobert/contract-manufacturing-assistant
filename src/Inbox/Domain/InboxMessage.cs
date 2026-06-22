namespace QuotationAccelerator.Inbox.Domain;

public sealed class InboxMessage
{
    public required string Id { get; init; }

    public required string GraphMessageId { get; init; }

    public required string Subject { get; init; }

    public required string FromAddress { get; init; }

    public string? FromDisplayName { get; init; }

    public required DateTimeOffset ReceivedAt { get; init; }

    public string? BodyPreview { get; init; }

    public string? BodyText { get; init; }

    public InboxMessageCategory Category { get; init; } = InboxMessageCategory.Uncategorized;

    public InboxMessageStatus Status { get; init; } = InboxMessageStatus.New;

    public IReadOnlyList<InboxAttachment> Attachments { get; init; } = [];

    public string? SuggestedReplyBody { get; init; }
}
