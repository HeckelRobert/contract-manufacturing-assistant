namespace QuotationAccelerator.Desktop.Services;

using QuotationAccelerator.Matching.Domain;

public sealed class NewContractManufacturingRequest
{
    public required AnalyzeInquiryResult Analysis { get; init; }

    public ProjectMatch? ReferenceProject { get; init; }
}
