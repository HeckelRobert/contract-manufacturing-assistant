namespace QuotationAccelerator.Desktop.Services.ContractManufacturing;

public sealed class ContractManufacturingTemplate
{
    public IReadOnlyList<string> ManufacturingSteps { get; init; } = [];

    public string ReferenceManufacturingStepsIntro { get; init; } = string.Empty;

    public string Quotation { get; init; } = string.Empty;

    public string ReferenceQuotationBlock { get; init; } = string.Empty;

    public string ReferencedDocuments { get; init; } = string.Empty;

    public string ReferenceDocumentsBlock { get; init; } = string.Empty;

    public string CustomerDrawingLine { get; init; } = string.Empty;

    public string NoCustomerDrawingLine { get; init; } = string.Empty;

    public string InquiryProcessLine { get; init; } = "• {ProcessName}";
}
