namespace QuotationAccelerator.Infrastructure.Mail;

using Microsoft.EntityFrameworkCore;
using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.Infrastructure.Persistence;

public sealed class FaqTemplateRepository(AppDbContext dbContext) : IFaqTemplateRepository
{
    public async Task<IReadOnlyList<FaqTemplate>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await dbContext.FaqTemplates.AsNoTracking().ToListAsync(cancellationToken);
        return entities.Select(Map).ToList();
    }

    public async Task SaveAllAsync(IReadOnlyList<FaqTemplate> templates, CancellationToken cancellationToken)
    {
        dbContext.FaqTemplates.RemoveRange(dbContext.FaqTemplates);
        foreach (var template in templates)
        {
            dbContext.FaqTemplates.Add(new FaqTemplateEntity
            {
                Id = string.IsNullOrWhiteSpace(template.Id) ? Guid.NewGuid().ToString("N") : template.Id,
                KeywordsJson = FaqTemplateJson.SerializeKeywords(template.Keywords),
                ReplyBody = template.ReplyBody,
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static FaqTemplate Map(FaqTemplateEntity entity) =>
        new()
        {
            Id = entity.Id,
            Keywords = FaqTemplateJson.DeserializeKeywords(entity.KeywordsJson),
            ReplyBody = entity.ReplyBody,
        };
}
