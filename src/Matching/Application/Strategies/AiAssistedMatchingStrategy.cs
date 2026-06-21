namespace QuotationAccelerator.Matching.Application.Strategies;

using QuotationAccelerator.Inquiry.Domain;
using QuotationAccelerator.Matching.Application.Abstractions;
using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.SharedKernel.Enums;

public sealed class AiAssistedMatchingStrategy : IMatchingStrategy
{
    public MatchingStrategy Strategy => MatchingStrategy.AiAssisted;

    public Task<IReadOnlyList<ProjectMatch>> MatchAsync(
        CustomerInquiry inquiry,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException(
            $"Strategy {Strategy} is not available until an AI provider or embedding model is configured.");
}
