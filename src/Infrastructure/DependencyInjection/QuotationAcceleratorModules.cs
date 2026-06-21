namespace QuotationAccelerator.Infrastructure.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using QuotationAccelerator.Catalog.Application.DependencyInjection;
using QuotationAccelerator.Matching.Application.DependencyInjection;

public static class QuotationAcceleratorModules
{
    public static IServiceCollection AddApplicationModules(this IServiceCollection services)
    {
        services.AddCatalogModule();
        services.AddMatchingModule();

        // Register future application modules here, e.g.:
        // services.AddProposalModule();
        // services.AddDocumentsModule();
        // services.AddExportModule();
        // services.AddSettingsModule();

        return services;
    }
}
