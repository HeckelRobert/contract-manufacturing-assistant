namespace QuotationAccelerator.Desktop.Services;

using System.Globalization;
using System.Resources;
using QuotationAccelerator.Desktop.Resources;
using QuotationAccelerator.SharedKernel.Enums;

public sealed class UiTextProvider : IUiTextProvider
{
    private static readonly ResourceManager ResourceManager = new(
        "QuotationAccelerator.Desktop.Resources.UiResources",
        typeof(UiResourceMarker).Assembly);

    public string Get(string key, UiLanguage language)
    {
        var culture = ToCulture(language);
        return ResourceManager.GetString(key, culture) ?? key;
    }

    public string Format(string key, UiLanguage language, params object[] args) =>
        string.Format(CultureInfo.GetCultureInfo(ToCulture(language).Name), Get(key, language), args);

    private static CultureInfo ToCulture(UiLanguage language) =>
        language switch
        {
            UiLanguage.German => new CultureInfo("de"),
            _ => CultureInfo.GetCultureInfo("en"),
        };
}
