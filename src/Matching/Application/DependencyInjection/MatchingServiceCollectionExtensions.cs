namespace QuotationAccelerator.Matching.Application.DependencyInjection;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using QuotationAccelerator.Matching.Application.Abstractions;
using QuotationAccelerator.Matching.Application.AnalyzeInquiry;
using QuotationAccelerator.Matching.Application.Resources;
using QuotationAccelerator.Matching.Application.Strategies;
using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Results;

public static class MatchingServiceCollectionExtensions
{
    public static IServiceCollection AddMatchingApplication(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AnalyzeInquiryCommand>, AnalyzeInquiryValidator>();
        services.AddScoped<ICommandHandler<AnalyzeInquiryCommand, Result<AnalyzeInquiryResult>>, AnalyzeInquiryHandler>();

        services.AddScoped<RuleBasedMatchingStrategy>();
        services.AddScoped<HybridMatchingStrategy>();
        services.AddScoped<AiAssistedMatchingStrategy>();
        services.AddScoped<IMatchingStrategyResolver, MatchingStrategyResolver>();
        services.AddScoped<IMatchingStrategy>(sp => sp.GetRequiredService<RuleBasedMatchingStrategy>());
        services.AddScoped<IMatchingStrategy>(sp => sp.GetRequiredService<HybridMatchingStrategy>());
        services.AddScoped<IMatchingStrategy>(sp => sp.GetRequiredService<AiAssistedMatchingStrategy>());

        services.AddSingleton<IMatchReasonFormatter, MatchReasonFormatter>();
        services.AddSingleton<IMatchingTextProvider, MatchingTextProvider>();

        return services;
    }
}
