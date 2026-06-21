namespace QuotationAccelerator.Catalog.Domain;

public sealed class ProjectMetadata
{
    public required string ProjectNumber { get; init; }

    public required string Title { get; init; }

    public required string Material { get; init; }

    public int Quantity { get; init; }

    public IReadOnlyList<string> Processes { get; init; } = [];

    public string? SurfaceTreatment { get; init; }

    public string? Customer { get; init; }
}
