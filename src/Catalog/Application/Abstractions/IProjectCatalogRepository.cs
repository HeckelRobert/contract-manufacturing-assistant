namespace QuotationAccelerator.Catalog.Application.Abstractions;

using QuotationAccelerator.Catalog.Domain;

public interface IProjectCatalogRepository
{
    Task ReplaceCatalogAsync(
        string projectRoot,
        IReadOnlyList<CatalogProject> projects,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CatalogProject>> GetAllAsync(CancellationToken cancellationToken);

    Task<int> GetCountAsync(CancellationToken cancellationToken);

    Task<string?> GetActiveProjectRootAsync(CancellationToken cancellationToken);

    Task SetActiveProjectRootAsync(string projectRoot, CancellationToken cancellationToken);
}
