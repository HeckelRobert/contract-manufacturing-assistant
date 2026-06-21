namespace QuotationAccelerator.Catalog.Application.RescanProjects;

using Microsoft.Extensions.Logging;
using QuotationAccelerator.Catalog.Application.Abstractions;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Results;

public sealed class RescanProjectsHandler(
    IProjectFolderScanner scanner,
    IProjectCatalogRepository repository,
    IAppPathProvider appPathProvider,
    ILogger<RescanProjectsHandler> logger) : ICommandHandler<RescanProjectsCommand, Result<RescanProjectsResult>>
{
    public async Task<Result<RescanProjectsResult>> HandleAsync(
        RescanProjectsCommand command,
        CancellationToken cancellationToken)
    {
        var projectRoot = ResolveProjectRoot(command.ProjectRoot, appPathProvider);
        if (string.IsNullOrWhiteSpace(projectRoot))
        {
            return Result<RescanProjectsResult>.Failure("Project root is not configured.");
        }

        if (!Directory.Exists(projectRoot))
        {
            return Result<RescanProjectsResult>.Failure($"Project root does not exist: {projectRoot}");
        }

        var projects = await scanner.ScanAsync(projectRoot, cancellationToken);
        await repository.ReplaceCatalogAsync(projectRoot, projects, cancellationToken);
        await repository.SetActiveProjectRootAsync(projectRoot, cancellationToken);

        logger.LogInformation(
            "Rescanned {ProjectCount} projects from {ProjectRoot}",
            projects.Count,
            projectRoot);

        return Result<RescanProjectsResult>.Success(new RescanProjectsResult(projects.Count, projectRoot));
    }

    private static string ResolveProjectRoot(string? requestedRoot, IAppPathProvider appPathProvider)
    {
        if (!string.IsNullOrWhiteSpace(requestedRoot))
        {
            return Path.GetFullPath(requestedRoot);
        }

        var configured = appPathProvider.GetConfiguredProjectRoot();
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return Path.GetFullPath(configured);
        }

        return Path.GetFullPath(appPathProvider.GetDefaultSampleDataPath());
    }
}
