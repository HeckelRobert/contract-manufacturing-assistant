namespace QuotationAccelerator.Desktop.Services;

using QuotationAccelerator.SharedKernel.Enums;

public interface IUiTextProvider
{
    string Get(string key, UiLanguage language);

    string Format(string key, UiLanguage language, params object[] args);
}
