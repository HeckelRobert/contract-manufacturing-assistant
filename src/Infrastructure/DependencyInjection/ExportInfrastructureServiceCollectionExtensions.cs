namespace QuotationAccelerator.Infrastructure.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using QuotationAccelerator.Export.Application.Abstractions;
using QuotationAccelerator.Export.Application.DependencyInjection;
using QuotationAccelerator.Infrastructure.Export;

public static class ExportInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddExportInfrastructure(this IServiceCollection services)
    {
        services.AddExportModule();
        services.AddSingleton<IProposalPdfExporter, QuestPdfProposalExporter>();
        services.AddSingleton<IProposalWordExporter, OpenXmlProposalWordExporter>();
        return services;
    }
}
