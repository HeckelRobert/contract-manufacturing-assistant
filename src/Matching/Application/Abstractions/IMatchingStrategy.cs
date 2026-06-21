namespace QuotationAccelerator.Matching.Application.Abstractions;

using QuotationAccelerator.Inquiry.Domain;
using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.SharedKernel.Enums;

public interface IMatchingStrategy
{
    MatchingStrategy Strategy { get; }

    Task<IReadOnlyList<ProjectMatch>> MatchAsync(
        CustomerInquiry inquiry,
        CancellationToken cancellationToken);
}
