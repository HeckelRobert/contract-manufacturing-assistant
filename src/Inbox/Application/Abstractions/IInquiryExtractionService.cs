namespace QuotationAccelerator.Inbox.Application.Abstractions;

using QuotationAccelerator.Inquiry.Domain;

public interface IInquiryExtractionService
{
    ExtractedInquiryFields Extract(string? subject, string? bodyText);
}

public sealed class ExtractedInquiryFields
{
    public int? Quantity { get; init; }

    public string? Material { get; init; }

    public string? SurfaceTreatment { get; init; }

    public string? PartDescription { get; init; }

    public string? DeliveryDeadline { get; init; }

    public string? SpecialRequirements { get; init; }

    public IReadOnlyList<string> ManufacturingProcesses { get; init; } = [];

    public string? WorkPreparationSummary { get; init; }

    public CustomerInquiry ToCustomerInquiry(
        string? drawingFilePath,
        string? notes,
        IReadOnlyList<string> availableMaterials,
        IReadOnlyList<string> availableSurfaceTreatments,
        IReadOnlyList<string> availableProcesses)
    {
        var material = ResolveMaterial(Material, availableMaterials);
        var surface = ResolveSurfaceTreatment(SurfaceTreatment, availableSurfaceTreatments);
        var processes = ManufacturingProcesses
            .Where(process => availableProcesses.Contains(process, StringComparer.OrdinalIgnoreCase))
            .ToList();

        return new CustomerInquiry
        {
            Material = material,
            Quantity = Quantity,
            SurfaceTreatment = surface,
            PartDescription = PartDescription,
            DeliveryDeadline = DeliveryDeadline,
            SpecialRequirements = SpecialRequirements,
            Notes = notes,
            DrawingFilePath = drawingFilePath,
            ManufacturingProcesses = processes,
        };
    }

    private static string ResolveMaterial(string? extracted, IReadOnlyList<string> available)
    {
        if (string.IsNullOrWhiteSpace(extracted))
        {
            return available.FirstOrDefault(m => m.Contains("S355", StringComparison.OrdinalIgnoreCase))
                ?? available.FirstOrDefault()
                ?? "S355";
        }

        return available.FirstOrDefault(m =>
                m.Contains(extracted, StringComparison.OrdinalIgnoreCase)
                || extracted.Contains(m, StringComparison.OrdinalIgnoreCase))
            ?? extracted;
    }

    private static string? ResolveSurfaceTreatment(string? extracted, IReadOnlyList<string> available)
    {
        if (string.IsNullOrWhiteSpace(extracted))
        {
            return available.FirstOrDefault();
        }

        if (extracted.Contains("verzink", StringComparison.OrdinalIgnoreCase))
        {
            return available.FirstOrDefault(t => t.Contains("Galvan", StringComparison.OrdinalIgnoreCase))
                ?? "Galvanized";
        }

        if (extracted.Contains("pulver", StringComparison.OrdinalIgnoreCase))
        {
            return available.FirstOrDefault(t => t.Contains("Powder", StringComparison.OrdinalIgnoreCase))
                ?? "Powder Coated";
        }

        return available.FirstOrDefault(t =>
                t.Contains(extracted, StringComparison.OrdinalIgnoreCase)
                || extracted.Contains(t, StringComparison.OrdinalIgnoreCase))
            ?? extracted;
    }
}
