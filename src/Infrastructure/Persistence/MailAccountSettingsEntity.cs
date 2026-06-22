namespace QuotationAccelerator.Infrastructure.Persistence;

public sealed class MailAccountSettingsEntity
{
    public int Id { get; set; } = 1;

    public string? TenantId { get; set; }

    public string? ClientId { get; set; }

    public string? MailboxAddress { get; set; }

    public string FolderName { get; set; } = "Inbox";

    public bool IsConnected { get; set; }

    public DateTimeOffset? LastFetchedAt { get; set; }
}
