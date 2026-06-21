namespace QuotationAccelerator.Catalog.Application.GetCatalogSummary;

using QuotationAccelerator.Catalog.Application.Abstractions;
using QuotationAccelerator.SharedKernel.Abstractions;

public sealed class GetCatalogSummaryHandler(
    IProjectCatalogRepository repository,
    IAppPathProvider appPathProvider) : IQueryHandler<GetCatalogSummaryQuery, CatalogSummaryResult>
{
    public async Task<CatalogSummaryResult> HandleAsync(
        GetCatalogSummaryQuery query,
        CancellationToken cancellationToken)
    {
        var count = await repository.GetCountAsync(cancellationToken);
        var root = await repository.GetActiveProjectRootAsync(cancellationToken)
            ?? appPathProvider.GetDefaultSampleDataPath();

        return new CatalogSummaryResult(count, root);
    }
}
