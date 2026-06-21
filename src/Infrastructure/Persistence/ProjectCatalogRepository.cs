namespace QuotationAccelerator.Infrastructure.Persistence;

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using QuotationAccelerator.Catalog.Application.Abstractions;
using QuotationAccelerator.Catalog.Domain;
using QuotationAccelerator.SharedKernel.Persistence;

public sealed class ProjectCatalogRepository(AppDbContext dbContext) : IProjectCatalogRepository
{

    public async Task ReplaceCatalogAsync(
        string projectRoot,
        IReadOnlyList<CatalogProject> projects,
        CancellationToken cancellationToken)
    {
        await dbContext.Projects.ExecuteDeleteAsync(cancellationToken);

        foreach (var project in projects)
        {
            dbContext.Projects.Add(new ProjectIndexEntity
            {
                ProjectNumber = project.Metadata.ProjectNumber,
                Title = project.Metadata.Title,
                Material = project.Metadata.Material,
                Quantity = project.Metadata.Quantity,
                ProcessesJson = JsonSerializer.Serialize(project.Metadata.Processes),
                SurfaceTreatment = project.Metadata.SurfaceTreatment,
                Customer = project.Metadata.Customer,
                FolderPath = project.FolderPath,
                FolderName = project.FolderName,
                DocumentFileNamesJson = JsonSerializer.Serialize(project.DocumentFileNames),
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CatalogProject>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await dbContext.Projects
            .AsNoTracking()
            .OrderBy(x => x.ProjectNumber)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToDomain).ToList();
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Projects.CountAsync(cancellationToken);
    }

    public async Task<string?> GetActiveProjectRootAsync(CancellationToken cancellationToken)
    {
        var setting = await dbContext.Settings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Key == ApplicationSettingKeys.ActiveProjectRoot, cancellationToken);

        return setting?.Value;
    }

    public async Task SetActiveProjectRootAsync(string projectRoot, CancellationToken cancellationToken)
    {
        var existing = await dbContext.Settings
            .FirstOrDefaultAsync(x => x.Key == ApplicationSettingKeys.ActiveProjectRoot, cancellationToken);

        if (existing is null)
        {
            dbContext.Settings.Add(new AppSettingEntity
            {
                Key = ApplicationSettingKeys.ActiveProjectRoot,
                Value = projectRoot,
            });
        }
        else
        {
            existing.Value = projectRoot;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static CatalogProject MapToDomain(ProjectIndexEntity entity)
    {
        return new CatalogProject
        {
            FolderPath = entity.FolderPath,
            FolderName = entity.FolderName,
            Metadata = new ProjectMetadata
            {
                ProjectNumber = entity.ProjectNumber,
                Title = entity.Title,
                Material = entity.Material,
                Quantity = entity.Quantity,
                Processes = JsonSerializer.Deserialize<List<string>>(entity.ProcessesJson) ?? [],
                SurfaceTreatment = entity.SurfaceTreatment,
                Customer = entity.Customer,
            },
            DocumentFileNames = JsonSerializer.Deserialize<List<string>>(entity.DocumentFileNamesJson) ?? [],
        };
    }
}
