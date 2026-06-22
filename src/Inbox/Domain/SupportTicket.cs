namespace QuotationAccelerator.Inbox.Domain;

public sealed class SupportTicket
{
    public required string Id { get; init; }

    public required string InboxMessageId { get; init; }

    public required string Subject { get; init; }

    public required string FromAddress { get; init; }

    public SupportTicketStatus Status { get; init; } = SupportTicketStatus.Open;

    public string? Notes { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset UpdatedAt { get; init; }
}
