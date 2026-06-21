namespace QuotationAccelerator.Infrastructure.DependencyInjection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuotationAccelerator.Catalog.Application.Abstractions;
using QuotationAccelerator.Catalog.Application.DependencyInjection;
using QuotationAccelerator.Catalog.Configuration;
using QuotationAccelerator.Infrastructure.Dispatching;
using QuotationAccelerator.Infrastructure.FileSystem;
using QuotationAccelerator.Infrastructure.Persistence;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Configuration;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddQuotationAcceleratorInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ApplicationOptions>(configuration.GetSection(ApplicationOptions.SectionName));
        services.Configure<CatalogOptions>(configuration.GetSection(CatalogOptions.SectionName));
        services.Configure<MatchingOptions>(configuration.GetSection(MatchingOptions.SectionName));
        services.Configure<AiOptions>(configuration.GetSection(AiOptions.SectionName));
        services.Configure<InquiryOptions>(configuration.GetSection(InquiryOptions.SectionName));

        services.AddSingleton<IAppPathProvider, AppPathProvider>();
        services.AddScoped<IProjectFolderScanner, ProjectFolderScanner>();
        services.AddScoped<IProjectCatalogRepository, ProjectCatalogRepository>();
        services.AddScoped<IDispatcher, Dispatcher>();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var paths = sp.GetRequiredService<IAppPathProvider>();
            options.UseSqlite($"Data Source={paths.GetDatabasePath()}");
        });

        services.AddApplicationModules();

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
        await ProjectCatalogSchemaPatcher.ApplyAsync(db, CancellationToken.None);
    }
}
