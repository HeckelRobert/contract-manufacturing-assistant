namespace QuotationAccelerator.Export.Application.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using QuotationAccelerator.Export.Application.Abstractions;
using QuotationAccelerator.Export.Application.Resources;

public static class ExportServiceCollectionExtensions
{
    public static IServiceCollection AddExportModule(this IServiceCollection services)
    {
        services.AddSingleton<IProposalExportTextProvider, ExportTextProvider>();
        services.AddSingleton<ProposalExportContentFactory>();
        return services;
    }
}
