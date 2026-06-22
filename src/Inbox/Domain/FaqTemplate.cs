namespace QuotationAccelerator.Inbox.Domain;

public sealed class FaqTemplate
{
    public required string Id { get; init; }

    public required IReadOnlyList<string> Keywords { get; init; }

    public required string ReplyBody { get; init; }
}
