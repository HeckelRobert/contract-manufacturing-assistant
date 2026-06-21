namespace QuotationAccelerator.Matching.Application.Strategies;

using System.Globalization;
using System.Resources;
using QuotationAccelerator.Matching.Application.Abstractions;
using QuotationAccelerator.Matching.Application.Resources;
using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.SharedKernel.Enums;

public sealed class MatchReasonFormatter : IMatchReasonFormatter
{
    private static readonly ResourceManager ResourceManager = new(
        "QuotationAccelerator.Matching.Application.Resources.MatchReasonResources",
        typeof(MatchReasonResourceMarker).Assembly);

    public string Format(MatchReasonCode reasonCode, UiLanguage language)
    {
        var culture = ToCulture(language);
        var key = reasonCode.ToString();
        return ResourceManager.GetString(key, culture) ?? key;
    }

    public IReadOnlyList<string> FormatAll(
        IEnumerable<MatchReasonCode> reasonCodes,
        UiLanguage language) =>
        reasonCodes.Select(code => Format(code, language)).ToList();

    private static CultureInfo ToCulture(UiLanguage language) =>
        language switch
        {
            UiLanguage.German => new CultureInfo("de"),
            _ => CultureInfo.GetCultureInfo("en"),
        };
}
