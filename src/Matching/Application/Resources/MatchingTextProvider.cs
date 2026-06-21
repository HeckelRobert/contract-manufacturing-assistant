namespace QuotationAccelerator.Matching.Application.Resources;

using System.Globalization;
using System.Resources;
using QuotationAccelerator.Matching.Application.Abstractions;
using QuotationAccelerator.SharedKernel.Enums;

public sealed class MatchingTextProvider : IMatchingTextProvider
{
    private static readonly ResourceManager ResourceManager = new(
        "QuotationAccelerator.Matching.Application.Resources.MatchingMessages",
        typeof(MatchingMessageResourceMarker).Assembly);

    public string Get(string key, UiLanguage language)
    {
        var culture = ToCulture(language);
        return ResourceManager.GetString(key, culture) ?? key;
    }

    private static CultureInfo ToCulture(UiLanguage language) =>
        language switch
        {
            UiLanguage.German => new CultureInfo("de"),
            _ => CultureInfo.GetCultureInfo("en"),
        };
}
