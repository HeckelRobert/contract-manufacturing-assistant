namespace QuotationAccelerator.Desktop.ViewModels;

using QuotationAccelerator.Inbox.Domain;

public sealed class InboxMessageItemViewModel
{
    public required string Id { get; init; }

    public required string Subject { get; init; }

    public required string FromDisplay { get; init; }

    public required string ReceivedDisplay { get; init; }

    public required string CategoryDisplay { get; init; }

    public required InboxMessageCategory Category { get; init; }

    public required string AttachmentsDisplay { get; init; }

    public string? BodyPreview { get; init; }

    public string? SuggestedReplyBody { get; init; }
}
