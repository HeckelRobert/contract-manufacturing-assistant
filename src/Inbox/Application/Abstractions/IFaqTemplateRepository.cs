namespace QuotationAccelerator.Inbox.Application.Abstractions;

using QuotationAccelerator.Inbox.Domain;

public interface IFaqTemplateRepository
{
    Task<IReadOnlyList<FaqTemplate>> GetAllAsync(CancellationToken cancellationToken);

    Task SaveAllAsync(IReadOnlyList<FaqTemplate> templates, CancellationToken cancellationToken);
}
