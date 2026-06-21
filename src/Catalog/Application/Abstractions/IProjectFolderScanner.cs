namespace QuotationAccelerator.Catalog.Application.Abstractions;

using QuotationAccelerator.Catalog.Domain;

public interface IProjectFolderScanner
{
    Task<IReadOnlyList<CatalogProject>> ScanAsync(string projectRoot, CancellationToken cancellationToken);
}
