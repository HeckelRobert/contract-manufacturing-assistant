namespace QuotationAccelerator.Export.Application.Resources;

using System.Globalization;
using System.Resources;
using QuotationAccelerator.Export.Application.Abstractions;
using QuotationAccelerator.SharedKernel.Enums;

public sealed class ExportTextProvider : IProposalExportTextProvider
{
    private static readonly ResourceManager ResourceManager = new(
        "QuotationAccelerator.Export.Application.Resources.ExportResources",
        typeof(ExportResourceMarker).Assembly);

    public string Get(string key, UiLanguage language) =>
        ResourceManager.GetString(key, ToCulture(language)) ?? key;

    public string Format(string key, UiLanguage language, params object[] args)
    {
        var format = Get(key, language);
        return string.Format(ToCulture(language), format, args);
    }

    private static CultureInfo ToCulture(UiLanguage language) =>
        language switch
        {
            UiLanguage.German => new CultureInfo("de"),
            _ => CultureInfo.GetCultureInfo("en"),
        };
}
