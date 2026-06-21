namespace QuotationAccelerator.Infrastructure.Persistence;

public sealed class ProjectIndexEntity
{
    public int Id { get; set; }

    public string ProjectNumber { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Material { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public string ProcessesJson { get; set; } = "[]";

    public string? SurfaceTreatment { get; set; }

    public string? Customer { get; set; }

    public string? PartDescription { get; set; }

    public string? DrawingNumber { get; set; }

    public string? Dimensions { get; set; }

    public string FolderPath { get; set; } = string.Empty;

    public string FolderName { get; set; } = string.Empty;

    public string DocumentFileNamesJson { get; set; } = "[]";
}
