namespace QuotationAccelerator.Matching.Application.Strategies;

using QuotationAccelerator.Matching.Application.Abstractions;
using QuotationAccelerator.SharedKernel.Enums;

public sealed class MatchingStrategyResolver(IEnumerable<IMatchingStrategy> strategies) : IMatchingStrategyResolver
{
    public IMatchingStrategy Resolve(MatchingStrategy strategy) =>
        strategies.FirstOrDefault(s => s.Strategy == strategy)
        ?? throw new InvalidOperationException($"No matching strategy registered for {strategy}.");
}
