namespace QuotationAccelerator.Matching.Domain;

using QuotationAccelerator.Inquiry.Domain;
using QuotationAccelerator.SharedKernel.Enums;

public sealed class AnalyzeInquiryResult
{
    public required CustomerInquiry Inquiry { get; init; }

    public required MatchingStrategy Strategy { get; init; }

    public required IReadOnlyList<ProjectMatch> Matches { get; init; }

    public ProjectMatch PrimaryMatch => Matches[0];
}
