namespace QuotationAccelerator.Infrastructure.FileSystem;

using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuotationAccelerator.Catalog.Application.Abstractions;
using QuotationAccelerator.Catalog.Configuration;
using QuotationAccelerator.Catalog.Domain;

public sealed class ProjectFolderScanner(
    IOptions<CatalogOptions> catalogOptions,
    ILogger<ProjectFolderScanner> logger) : IProjectFolderScanner
{
    public Task<IReadOnlyList<CatalogProject>> ScanAsync(string projectRoot, CancellationToken cancellationToken)
    {
        var options = catalogOptions.Value;
        var projects = new List<CatalogProject>();

        if (!Directory.Exists(projectRoot))
        {
            logger.LogWarning("Project root not found: {ProjectRoot}", projectRoot);
            return Task.FromResult<IReadOnlyList<CatalogProject>>(projects);
        }

        foreach (var folder in Directory.EnumerateDirectories(projectRoot).OrderBy(x => x))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var folderName = Path.GetFileName(folder);
            if (!folderName.StartsWith(options.ProjectFolderPrefix, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var metadataPath = Path.Combine(folder, options.MetadataFileName);
            if (!File.Exists(metadataPath))
            {
                logger.LogWarning(
                    "Skipping {FolderName}: {MetadataFileName} not found",
                    folderName,
                    options.MetadataFileName);
                continue;
            }

            try
            {
                var json = File.ReadAllText(metadataPath);
                var metadata = JsonSerializer.Deserialize<ProjectMetadataDto>(json, JsonSerializerOptions.Web);
                if (metadata is null)
                {
                    continue;
                }

                var documents = Directory.EnumerateFiles(folder)
                    .Select(Path.GetFileName)
                    .Where(name => name is not null)
                    .Cast<string>()
                    .OrderBy(name => name)
                    .ToList();

                projects.Add(new CatalogProject
                {
                    FolderPath = folder,
                    FolderName = folderName,
                    Metadata = metadata.ToDomain(),
                    DocumentFileNames = documents,
                });
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to read metadata for {FolderName}", folderName);
            }
        }

        return Task.FromResult<IReadOnlyList<CatalogProject>>(projects);
    }

    private sealed class ProjectMetadataDto
    {
        public string ProjectNumber { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Material { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public List<string> Processes { get; set; } = [];

        public string? SurfaceTreatment { get; set; }

        public string? Customer { get; set; }

        public string? PartDescription { get; set; }

        public string? DrawingNumber { get; set; }

        public string? Dimensions { get; set; }

        public ProjectMetadata ToDomain() => new()
        {
            ProjectNumber = ProjectNumber,
            Title = Title,
            Material = Material,
            Quantity = Quantity,
            Processes = Processes,
            SurfaceTreatment = SurfaceTreatment,
            Customer = Customer,
            PartDescription = PartDescription,
            DrawingNumber = DrawingNumber,
            Dimensions = Dimensions,
        };
    }
}
