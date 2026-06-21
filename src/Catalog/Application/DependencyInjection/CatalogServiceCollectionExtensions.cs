namespace QuotationAccelerator.Catalog.Application.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using QuotationAccelerator.Catalog.Application.GetCatalogSummary;
using QuotationAccelerator.Catalog.Application.RescanProjects;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Results;

public static class CatalogServiceCollectionExtensions
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<RescanProjectsCommand, Result<RescanProjectsResult>>, RescanProjectsHandler>();
        services.AddScoped<IQueryHandler<GetCatalogSummaryQuery, CatalogSummaryResult>, GetCatalogSummaryHandler>();
        return services;
    }
}
