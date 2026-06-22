namespace QuotationAccelerator.Inbox.Domain;

public sealed class MailAccountSettings
{
    public string? TenantId { get; init; }

    public string? ClientId { get; init; }

    public string? MailboxAddress { get; init; }

    public string FolderName { get; init; } = "Inbox";

    public bool IsConnected { get; init; }

    public DateTimeOffset? LastFetchedAt { get; init; }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(TenantId)
        && !string.IsNullOrWhiteSpace(ClientId)
        && !string.IsNullOrWhiteSpace(MailboxAddress);
}
