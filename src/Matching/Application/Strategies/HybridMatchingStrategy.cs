namespace QuotationAccelerator.Matching.Application.Strategies;

using QuotationAccelerator.Inquiry.Domain;
using QuotationAccelerator.Matching.Application.Abstractions;
using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.SharedKernel.Enums;

public sealed class HybridMatchingStrategy(RuleBasedMatchingStrategy ruleBasedStrategy) : IMatchingStrategy
{
    public MatchingStrategy Strategy => MatchingStrategy.Hybrid;

    public Task<IReadOnlyList<ProjectMatch>> MatchAsync(
        CustomerInquiry inquiry,
        CancellationToken cancellationToken) =>
        ruleBasedStrategy.MatchAsync(inquiry, cancellationToken);
}
