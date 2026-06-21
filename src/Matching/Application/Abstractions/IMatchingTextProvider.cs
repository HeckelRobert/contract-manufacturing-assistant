namespace QuotationAccelerator.Matching.Application.Abstractions;

using QuotationAccelerator.SharedKernel.Enums;

public interface IMatchingTextProvider
{
    string Get(string key, UiLanguage language);
}
