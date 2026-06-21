namespace QuotationAccelerator.Matching.Application.Abstractions;

using QuotationAccelerator.SharedKernel.Enums;

public interface IMatchingStrategyResolver
{
    IMatchingStrategy Resolve(MatchingStrategy strategy);
}
