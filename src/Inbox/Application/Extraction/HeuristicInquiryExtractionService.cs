namespace QuotationAccelerator.Inbox.Application.Extraction;

using System.Text.RegularExpressions;
using QuotationAccelerator.Inbox.Application.Abstractions;

public sealed partial class HeuristicInquiryExtractionService : IInquiryExtractionService
{
    public ExtractedInquiryFields Extract(string? subject, string? bodyText)
    {
        var combined = $"{subject}\n{bodyText}";
        if (string.IsNullOrWhiteSpace(combined))
        {
            return new ExtractedInquiryFields();
        }

        return new ExtractedInquiryFields
        {
            Quantity = ExtractQuantity(combined),
            Material = ExtractMaterial(combined),
            SurfaceTreatment = ExtractSurfaceTreatment(combined),
            PartDescription = ExtractPartDescription(combined, subject),
            DeliveryDeadline = ExtractDeliveryDeadline(combined),
            SpecialRequirements = ExtractSpecialRequirements(combined),
            ManufacturingProcesses = ExtractProcesses(combined),
            WorkPreparationSummary = ExtractWorkPreparationSummary(combined),
        };
    }

    private static int? ExtractQuantity(string text)
    {
        var match = QuantityRegex().Match(text);
        return match.Success && int.TryParse(match.Groups[1].Value, out var quantity)
            ? quantity
            : null;
    }

    private static string? ExtractMaterial(string text)
    {
        var labeled = MaterialLabelRegex().Match(text);
        if (labeled.Success)
        {
            return labeled.Groups[1].Value.Trim();
        }

        var steel = SteelGradeRegex().Match(text);
        return steel.Success ? steel.Value : null;
    }

    private static string? ExtractSurfaceTreatment(string text)
    {
        var labeled = SurfaceLabelRegex().Match(text);
        if (labeled.Success)
        {
            return labeled.Groups[1].Value.Trim();
        }

        if (text.Contains("verzink", StringComparison.OrdinalIgnoreCase))
        {
            return "verzinkt";
        }

        if (text.Contains("pulver", StringComparison.OrdinalIgnoreCase))
        {
            return "Pulverbeschichtung";
        }

        return null;
    }

    private static string? ExtractPartDescription(string text, string? subject)
    {
        var example = PartDescriptionRegex().Match(text);
        if (example.Success)
        {
            return example.Groups[1].Value.Trim();
        }

        return string.IsNullOrWhiteSpace(subject) ? null : subject.Trim();
    }

    private static string? ExtractDeliveryDeadline(string text)
    {
        var labeled = DeliveryLabelRegex().Match(text);
        return labeled.Success ? labeled.Groups[1].Value.Trim() : null;
    }

    private static string? ExtractSpecialRequirements(string text)
    {
        var labeled = SpecialRequirementsRegex().Match(text);
        return labeled.Success ? labeled.Groups[1].Value.Trim() : null;
    }

    private static IReadOnlyList<string> ExtractProcesses(string text)
    {
        var processes = new List<string>();
        if (text.Contains("laser", StringComparison.OrdinalIgnoreCase)
            || text.Contains("schneid", StringComparison.OrdinalIgnoreCase))
        {
            processes.Add("Laser Cutting");
        }

        if (text.Contains("bieg", StringComparison.OrdinalIgnoreCase)
            || text.Contains("kant", StringComparison.OrdinalIgnoreCase))
        {
            processes.Add("Bending");
        }

        if (text.Contains("schwei", StringComparison.OrdinalIgnoreCase))
        {
            processes.Add("Welding");
        }

        if (text.Contains("fräs", StringComparison.OrdinalIgnoreCase)
            || text.Contains("fraes", StringComparison.OrdinalIgnoreCase))
        {
            processes.Add("Milling");
        }

        if (text.Contains("dreh", StringComparison.OrdinalIgnoreCase))
        {
            processes.Add("Turning");
        }

        if (text.Contains("lack", StringComparison.OrdinalIgnoreCase)
            || text.Contains("beschicht", StringComparison.OrdinalIgnoreCase))
        {
            processes.Add("Painting");
        }

        return processes;
    }

    private static string? ExtractWorkPreparationSummary(string text)
    {
        var thickness = ThicknessRegex().Match(text);
        var length = LengthRegex().Match(text);
        var holes = HolesRegex().Match(text);
        var bend = BendAngleRegex().Match(text);
        var tolerance = ToleranceRegex().Match(text);

        var parts = new List<string>();
        if (thickness.Success)
        {
            parts.Add($"Thickness: {thickness.Groups[1].Value.Trim()} mm");
        }

        if (length.Success)
        {
            parts.Add($"Length: {length.Groups[1].Value.Trim()} mm");
        }

        if (holes.Success)
        {
            parts.Add($"Holes: {holes.Groups[1].Value.Trim()}");
        }

        if (bend.Success)
        {
            parts.Add($"Bend angle: {bend.Groups[1].Value.Trim()}°");
        }

        if (tolerance.Success)
        {
            parts.Add($"Tolerance: {tolerance.Groups[1].Value.Trim()}");
        }

        return parts.Count == 0 ? null : string.Join(Environment.NewLine, parts);
    }

    [GeneratedRegex(@"(?i)(\d+)\s*(?:stück|stueck|pcs|pieces?)")]
    private static partial Regex QuantityRegex();

    [GeneratedRegex(@"(?i)material\s*:\s*([^\r\n]+)")]
    private static partial Regex MaterialLabelRegex();

    [GeneratedRegex(@"(?i)\bS\d{3}(?:JR)?\b|\b1\.\d{4}\b|\bEN\s*AW[-\s]?\d{4}\b")]
    private static partial Regex SteelGradeRegex();

    [GeneratedRegex(@"(?i)(?:oberfläche|oberflaeche|surface)\s*:\s*([^\r\n]+)")]
    private static partial Regex SurfaceLabelRegex();

    [GeneratedRegex(@"(?i)(\d+\s+stück[^\r\n]+)")]
    private static partial Regex PartDescriptionRegex();

    [GeneratedRegex(@"(?i)(?:liefertermin|delivery)\s*:\s*([^\r\n]+)")]
    private static partial Regex DeliveryLabelRegex();

    [GeneratedRegex(@"(?i)(?:sonder|special)\s*(?:anforderung|requirement)[^\r\n:]*:\s*([^\r\n]+)")]
    private static partial Regex SpecialRequirementsRegex();

    [GeneratedRegex(@"(?i)(?:dicke|thickness)\s*[:=]?\s*(\d+(?:[.,]\d+)?)\s*mm")]
    private static partial Regex ThicknessRegex();

    [GeneratedRegex(@"(?i)(?:länge|laenge|length)\s*[:=]?\s*(\d+(?:[.,]\d+)?)\s*mm")]
    private static partial Regex LengthRegex();

    [GeneratedRegex(@"(?i)(\d+\s*(?:stück|stueck)?\s*Ø?\s*\d+(?:[.,]\d+)?\s*mm)")]
    private static partial Regex HolesRegex();

    [GeneratedRegex(@"(?i)(\d{1,3})\s*°")]
    private static partial Regex BendAngleRegex();

    [GeneratedRegex(@"(?i)(±\s*\d+(?:[.,]\d+)?\s*mm)")]
    private static partial Regex ToleranceRegex();
}
