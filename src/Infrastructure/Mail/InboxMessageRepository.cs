namespace QuotationAccelerator.Infrastructure.Mail;

using Microsoft.EntityFrameworkCore;
using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.Infrastructure.Persistence;

public sealed class InboxMessageRepository(AppDbContext dbContext) : IInboxMessageRepository
{
    public async Task<IReadOnlyList<InboxMessage>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await dbContext.InboxMessages
            .AsNoTracking()
            .Include(message => message.Attachments)
            .ToListAsync(cancellationToken);

        return entities
            .OrderByDescending(message => message.ReceivedAt)
            .Select(Map)
            .ToList();
    }

    public async Task<InboxMessage?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.InboxMessages
            .AsNoTracking()
            .Include(message => message.Attachments)
            .FirstOrDefaultAsync(message => message.Id == id, cancellationToken);

        return entity is null ? null : Map(entity);
    }

    public async Task<InboxMessage?> GetByGraphMessageIdAsync(string graphMessageId, CancellationToken cancellationToken)
    {
        var entity = await dbContext.InboxMessages
            .AsNoTracking()
            .Include(message => message.Attachments)
            .FirstOrDefaultAsync(message => message.GraphMessageId == graphMessageId, cancellationToken);

        return entity is null ? null : Map(entity);
    }

    public async Task UpsertAsync(InboxMessage message, CancellationToken cancellationToken)
    {
        var entity = await dbContext.InboxMessages
            .Include(item => item.Attachments)
            .FirstOrDefaultAsync(item => item.Id == message.Id, cancellationToken);

        if (entity is null)
        {
            entity = new InboxMessageEntity
            {
                Id = message.Id,
                GraphMessageId = message.GraphMessageId,
                Subject = message.Subject,
                FromAddress = message.FromAddress,
            };
            dbContext.InboxMessages.Add(entity);
        }
        else
        {
            dbContext.InboxAttachments.RemoveRange(entity.Attachments);
            entity.Attachments.Clear();
        }

        entity.GraphMessageId = message.GraphMessageId;
        entity.Subject = message.Subject;
        entity.FromAddress = message.FromAddress;
        entity.FromDisplayName = message.FromDisplayName;
        entity.ReceivedAt = message.ReceivedAt;
        entity.BodyPreview = message.BodyPreview;
        entity.BodyText = message.BodyText;
        entity.Category = (int)message.Category;
        entity.Status = (int)message.Status;
        entity.SuggestedReplyBody = message.SuggestedReplyBody;

        foreach (var attachment in message.Attachments)
        {
            entity.Attachments.Add(new InboxAttachmentEntity
            {
                Id = attachment.Id,
                InboxMessageId = message.Id,
                FileName = attachment.FileName,
                ContentType = attachment.ContentType,
                LocalPath = attachment.LocalPath,
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateCategoryAsync(
        string id,
        InboxMessageCategory category,
        string? suggestedReplyBody,
        CancellationToken cancellationToken)
    {
        var entity = await dbContext.InboxMessages.FirstAsync(message => message.Id == id, cancellationToken);
        entity.Category = (int)category;
        entity.SuggestedReplyBody = suggestedReplyBody;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateStatusAsync(string id, InboxMessageStatus status, CancellationToken cancellationToken)
    {
        var entity = await dbContext.InboxMessages.FirstAsync(message => message.Id == id, cancellationToken);
        entity.Status = (int)status;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAttachmentLocalPathAsync(
        string messageId,
        string attachmentId,
        string localPath,
        CancellationToken cancellationToken)
    {
        var entity = await dbContext.InboxAttachments.FirstAsync(
            attachment => attachment.Id == attachmentId && attachment.InboxMessageId == messageId,
            cancellationToken);
        entity.LocalPath = localPath;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> ClearAllAsync(CancellationToken cancellationToken)
    {
        var deletedCount = await dbContext.InboxMessages.CountAsync(cancellationToken);
        await dbContext.InboxAttachments.ExecuteDeleteAsync(cancellationToken);
        await dbContext.InboxMessages.ExecuteDeleteAsync(cancellationToken);
        return deletedCount;
    }

    private static InboxMessage Map(InboxMessageEntity entity) =>
        new()
        {
            Id = entity.Id,
            GraphMessageId = entity.GraphMessageId,
            Subject = entity.Subject,
            FromAddress = entity.FromAddress,
            FromDisplayName = entity.FromDisplayName,
            ReceivedAt = entity.ReceivedAt,
            BodyPreview = entity.BodyPreview,
            BodyText = entity.BodyText,
            Category = (InboxMessageCategory)entity.Category,
            Status = (InboxMessageStatus)entity.Status,
            SuggestedReplyBody = entity.SuggestedReplyBody,
            Attachments = entity.Attachments
                .Select(attachment => new InboxAttachment
                {
                    Id = attachment.Id,
                    FileName = attachment.FileName,
                    ContentType = attachment.ContentType,
                    LocalPath = attachment.LocalPath,
                })
                .ToList(),
        };
}
