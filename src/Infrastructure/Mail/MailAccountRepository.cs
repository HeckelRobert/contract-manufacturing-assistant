namespace QuotationAccelerator.Infrastructure.Mail;

using Microsoft.EntityFrameworkCore;
using QuotationAccelerator.Catalog.Application.Abstractions;
using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.Infrastructure.Persistence;

public sealed class MailAccountRepository(AppDbContext dbContext) : IMailAccountRepository
{
    public async Task<MailAccountSettings> GetSettingsAsync(CancellationToken cancellationToken)
    {
        var entity = await dbContext.MailAccountSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == 1, cancellationToken);

        if (entity is null)
        {
            return new MailAccountSettings();
        }

        return Map(entity);
    }

    public async Task SaveSettingsAsync(MailAccountSettings settings, CancellationToken cancellationToken)
    {
        var entity = await dbContext.MailAccountSettings.FirstOrDefaultAsync(item => item.Id == 1, cancellationToken)
            ?? new MailAccountSettingsEntity { Id = 1 };

        entity.TenantId = settings.TenantId;
        entity.ClientId = settings.ClientId;
        entity.MailboxAddress = settings.MailboxAddress;
        entity.FolderName = settings.FolderName;
        entity.IsConnected = settings.IsConnected;
        entity.LastFetchedAt = settings.LastFetchedAt;

        if (dbContext.Entry(entity).State == EntityState.Detached)
        {
            dbContext.MailAccountSettings.Add(entity);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SetConnectedAsync(bool isConnected, CancellationToken cancellationToken)
    {
        var entity = await dbContext.MailAccountSettings.FirstAsync(item => item.Id == 1, cancellationToken);
        entity.IsConnected = isConnected;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SetLastFetchedAtAsync(DateTimeOffset fetchedAt, CancellationToken cancellationToken)
    {
        var entity = await dbContext.MailAccountSettings.FirstAsync(item => item.Id == 1, cancellationToken);
        entity.LastFetchedAt = fetchedAt;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ClearLastFetchedAtAsync(CancellationToken cancellationToken)
    {
        var entity = await dbContext.MailAccountSettings.FirstAsync(item => item.Id == 1, cancellationToken);
        entity.LastFetchedAt = null;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static MailAccountSettings Map(MailAccountSettingsEntity entity) =>
        new()
        {
            TenantId = entity.TenantId,
            ClientId = entity.ClientId,
            MailboxAddress = entity.MailboxAddress,
            FolderName = entity.FolderName,
            IsConnected = entity.IsConnected,
            LastFetchedAt = entity.LastFetchedAt,
        };
}
