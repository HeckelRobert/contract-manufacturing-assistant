namespace QuotationAccelerator.Catalog.Configuration;

using QuotationAccelerator.Catalog.Domain;

public sealed class CatalogOptions
{
    public const string SectionName = "Catalog";

    public string ProjectFolderPrefix { get; set; } = ProjectCatalogConventions.FolderNamePrefix;

    public string MetadataFileName { get; set; } = ProjectCatalogFileNames.Metadata;
}
