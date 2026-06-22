namespace QuotationAccelerator.Inbox.Application.GetFaqTemplates;

using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.SharedKernel.Abstractions;

public sealed record GetFaqTemplatesQuery : IQuery<IReadOnlyList<FaqTemplate>>;

public sealed class GetFaqTemplatesHandler(IFaqTemplateRepository repository)
    : IQueryHandler<GetFaqTemplatesQuery, IReadOnlyList<FaqTemplate>>
{
    public Task<IReadOnlyList<FaqTemplate>> HandleAsync(
        GetFaqTemplatesQuery query,
        CancellationToken cancellationToken) =>
        repository.GetAllAsync(cancellationToken);
}
