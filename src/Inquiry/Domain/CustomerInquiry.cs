namespace QuotationAccelerator.Inquiry.Domain;

public sealed class CustomerInquiry
{
    public required string Material { get; init; }

    public int? Quantity { get; init; }

    public string? SurfaceTreatment { get; init; }

    public string? PartDescription { get; init; }

    public string? DeliveryDeadline { get; init; }

    public string? SpecialRequirements { get; init; }

    public IReadOnlyList<string> ManufacturingProcesses { get; init; } = [];

    public string? Notes { get; init; }

    public string? DrawingFilePath { get; init; }

    public bool HasDrawing => !string.IsNullOrWhiteSpace(DrawingFilePath);

    public bool HasPartDescription => !string.IsNullOrWhiteSpace(PartDescription);
}
