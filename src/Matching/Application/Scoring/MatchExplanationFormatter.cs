namespace QuotationAccelerator.Matching.Application.Scoring;

using System.Globalization;
using System.Resources;
using QuotationAccelerator.Catalog.Domain;
using QuotationAccelerator.Inquiry.Domain;
using QuotationAccelerator.Matching.Application.Abstractions;
using QuotationAccelerator.Matching.Application.Resources;
using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.SharedKernel.Enums;

public sealed class MatchExplanationFormatter : IMatchExplanationFormatter
{
    private static readonly ResourceManager ResourceManager = new(
        "QuotationAccelerator.Matching.Application.Resources.MatchReasonResources",
        typeof(MatchReasonResourceMarker).Assembly);

    public IReadOnlyList<string> FormatExplanations(
        CustomerInquiry inquiry,
        ProjectMatch match,
        UiLanguage language)
    {
        var culture = ToCulture(language);
        var metadata = match.Project.Metadata;
        var explanations = new List<string>();

        foreach (var reason in match.Reasons)
        {
            var text = reason switch
            {
                MatchReasonCode.SameMaterial => Format(
                    culture,
                    nameof(MatchReasonCode.SameMaterial) + "Detail",
                    inquiry.Material,
                    metadata.Material),

                MatchReasonCode.SameSurfaceTreatment => Format(
                    culture,
                    nameof(MatchReasonCode.SameSurfaceTreatment) + "Detail",
                    inquiry.SurfaceTreatment ?? "-",
                    metadata.SurfaceTreatment ?? "-"),

                MatchReasonCode.SharedManufacturingProcess => Format(
                    culture,
                    nameof(MatchReasonCode.SharedManufacturingProcess) + "Detail",
                    string.Join(", ", GetSharedProcesses(inquiry, metadata))),

                MatchReasonCode.SimilarQuantity => Format(
                    culture,
                    nameof(MatchReasonCode.SimilarQuantity) + "Detail",
                    inquiry.Quantity ?? 0,
                    metadata.Quantity),

                MatchReasonCode.KeywordMatchInTitle => Format(
                    culture,
                    nameof(MatchReasonCode.KeywordMatchInTitle) + "Detail",
                    inquiry.PartDescription ?? "-",
                    metadata.Title),

                MatchReasonCode.ComparableAssembly => Format(
                    culture,
                    nameof(MatchReasonCode.ComparableAssembly) + "Detail",
                    metadata.Title,
                    metadata.PartDescription ?? metadata.Title),

                MatchReasonCode.ContainsWeldingOperations => GetString(
                    culture,
                    nameof(MatchReasonCode.ContainsWeldingOperations) + "Detail"),

                MatchReasonCode.ExistingBendingSetup => GetString(
                    culture,
                    nameof(MatchReasonCode.ExistingBendingSetup) + "Detail"),

                MatchReasonCode.PartDescriptionSimilarity => Format(
                    culture,
                    nameof(MatchReasonCode.PartDescriptionSimilarity) + "Detail",
                    inquiry.PartDescription ?? "-",
                    metadata.PartDescription ?? "-"),

                _ => reason.ToString(),
            };

            if (!string.IsNullOrWhiteSpace(text))
            {
                explanations.Add(text);
            }
        }

        return explanations;
    }

    private static IEnumerable<string> GetSharedProcesses(CustomerInquiry inquiry, ProjectMetadata metadata) =>
        inquiry.ManufacturingProcesses
            .Intersect(metadata.Processes, StringComparer.OrdinalIgnoreCase)
            .OrderBy(process => process);

    private static string Format(CultureInfo culture, string key, params object[] args)
    {
        var format = ResourceManager.GetString(key, culture);
        return format is null ? key : string.Format(culture, format, args);
    }

    private static string GetString(CultureInfo culture, string key) =>
        ResourceManager.GetString(key, culture) ?? key;

    private static CultureInfo ToCulture(UiLanguage language) =>
        language switch
        {
            UiLanguage.German => new CultureInfo("de"),
            _ => CultureInfo.GetCultureInfo("en"),
        };
}
