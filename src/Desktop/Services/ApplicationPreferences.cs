namespace QuotationAccelerator.Desktop.Services;

using QuotationAccelerator.SharedKernel.Enums;

public sealed class ApplicationPreferences
{
    public MatchingStrategy MatchingStrategy { get; set; } = MatchingStrategy.Hybrid;

    public UiLanguage UiLanguage { get; set; } = UiLanguage.German;
}
