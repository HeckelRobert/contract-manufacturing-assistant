namespace QuotationAccelerator.SharedKernel.Configuration;

public sealed class InquiryOptions
{
    public const string SectionName = "Inquiry";

    public IReadOnlyList<string> Materials { get; set; } = [];

    public IReadOnlyList<string> SurfaceTreatments { get; set; } = [];

    public IReadOnlyList<string> ManufacturingProcesses { get; set; } = [];

    public IReadOnlyList<string> PartDescriptionExamples { get; set; } = [];
}
