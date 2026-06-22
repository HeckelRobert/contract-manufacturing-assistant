namespace QuotationAccelerator.Inbox.Application.Abstractions;

using QuotationAccelerator.Inbox.Domain;

public interface IMailAccountRepository
{
    Task<MailAccountSettings> GetSettingsAsync(CancellationToken cancellationToken);

    Task SaveSettingsAsync(MailAccountSettings settings, CancellationToken cancellationToken);

    Task SetConnectedAsync(bool isConnected, CancellationToken cancellationToken);

    Task SetLastFetchedAtAsync(DateTimeOffset fetchedAt, CancellationToken cancellationToken);

    Task ClearLastFetchedAtAsync(CancellationToken cancellationToken);
}
