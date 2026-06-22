namespace QuotationAccelerator.Infrastructure.Mail;

using Microsoft.EntityFrameworkCore;
using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.Infrastructure.Persistence;

public sealed class SupportTicketRepository(AppDbContext dbContext) : ISupportTicketRepository
{
    public async Task<IReadOnlyList<SupportTicket>> GetOpenAsync(CancellationToken cancellationToken)
    {
        var entities = await dbContext.SupportTickets
            .AsNoTracking()
            .Where(ticket => ticket.Status != (int)SupportTicketStatus.Resolved)
            .ToListAsync(cancellationToken);

        return entities
            .OrderByDescending(ticket => ticket.UpdatedAt)
            .Select(Map)
            .ToList();
    }

    public async Task<IReadOnlyList<SupportTicket>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await dbContext.SupportTickets
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return entities
            .OrderByDescending(ticket => ticket.UpdatedAt)
            .Select(Map)
            .ToList();
    }

    public async Task<SupportTicket?> GetByInboxMessageIdAsync(
        string inboxMessageId,
        CancellationToken cancellationToken)
    {
        var entity = await dbContext.SupportTickets
            .AsNoTracking()
            .FirstOrDefaultAsync(ticket => ticket.InboxMessageId == inboxMessageId, cancellationToken);

        return entity is null ? null : Map(entity);
    }

    public async Task UpsertAsync(SupportTicket ticket, CancellationToken cancellationToken)
    {
        var entity = await dbContext.SupportTickets.FirstOrDefaultAsync(item => item.Id == ticket.Id, cancellationToken);
        if (entity is null)
        {
            entity = new SupportTicketEntity
            {
                Id = ticket.Id,
                InboxMessageId = ticket.InboxMessageId,
                Subject = ticket.Subject,
                FromAddress = ticket.FromAddress,
            };
            dbContext.SupportTickets.Add(entity);
        }

        entity.InboxMessageId = ticket.InboxMessageId;
        entity.Subject = ticket.Subject;
        entity.FromAddress = ticket.FromAddress;
        entity.Status = (int)ticket.Status;
        entity.Notes = ticket.Notes;
        entity.CreatedAt = ticket.CreatedAt;
        entity.UpdatedAt = ticket.UpdatedAt;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ClearAllAsync(CancellationToken cancellationToken) =>
        await dbContext.SupportTickets.ExecuteDeleteAsync(cancellationToken);

    private static SupportTicket Map(SupportTicketEntity entity) =>
        new()
        {
            Id = entity.Id,
            InboxMessageId = entity.InboxMessageId,
            Subject = entity.Subject,
            FromAddress = entity.FromAddress,
            Status = (SupportTicketStatus)entity.Status,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
}
