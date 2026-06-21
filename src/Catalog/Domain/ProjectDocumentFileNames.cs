namespace QuotationAccelerator.Catalog.Domain;

public static class ProjectDocumentFileNames
{
    public const string Drawing = "Drawing.pdf";

    public const string Offer = "Offer.pdf";

    public const string Calculation = "Calculation.pdf";

    public const string WorkInstruction = "WorkInstruction.pdf";

    public const string Fixture = "Fixture.pdf";

    public const string CncProgram = "CncProgram.pdf";

    public const string InspectionReport = "InspectionReport.pdf";

    public static IReadOnlyList<string> StandardDocuments { get; } =
    [
        Drawing,
        Offer,
        Calculation,
        WorkInstruction,
        Fixture,
        CncProgram,
        InspectionReport,
    ];
}
