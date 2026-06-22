namespace QuotationAccelerator.Infrastructure.Persistence;

public sealed class SupportTicketEntity
{
    public required string Id { get; set; }

    public required string InboxMessageId { get; set; }

    public required string Subject { get; set; }

    public required string FromAddress { get; set; }

    public int Status { get; set; }

    public string? Notes { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
