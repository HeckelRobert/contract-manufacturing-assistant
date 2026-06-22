namespace QuotationAccelerator.Desktop.Resources;

public static class InquiryOptionLabels
{
    private static readonly IReadOnlyDictionary<string, string> SurfaceTreatmentKeys =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["None"] = UiTextKeys.SurfaceTreatment_None,
            ["Powder Coated"] = UiTextKeys.SurfaceTreatment_PowderCoated,
            ["Wet Paint"] = UiTextKeys.SurfaceTreatment_WetPaint,
            ["Galvanized"] = UiTextKeys.SurfaceTreatment_Galvanized,
            ["Anodized"] = UiTextKeys.SurfaceTreatment_Anodized,
        };

    public static string GetSurfaceTreatmentResourceKey(string value) =>
        SurfaceTreatmentKeys.TryGetValue(value, out var key) ? key : value;

    public static string GetPartDescriptionExampleLabelKey(string exampleKey) =>
        $"PartDescriptionExample_{exampleKey}";

    public static string GetPartDescriptionExampleTextKey(string exampleKey) =>
        $"PartDescriptionExample_{exampleKey}_Description";
}
