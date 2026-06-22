namespace QuotationAccelerator.Desktop.ViewModels;

using QuotationAccelerator.Inbox.Domain;

public sealed class SupportTicketItemViewModel
{
    public required string Id { get; init; }

    public required string Subject { get; init; }

    public required string FromAddress { get; init; }

    public required string StatusDisplay { get; init; }

    public required SupportTicketStatus Status { get; init; }

    public string? Notes { get; init; }
}
