namespace QuotationAccelerator.Catalog.Domain;

public sealed class CatalogProject
{
    public required string FolderPath { get; init; }

    public required string FolderName { get; init; }

    public required ProjectMetadata Metadata { get; init; }

    public IReadOnlyList<string> DocumentFileNames { get; init; } = [];
}
