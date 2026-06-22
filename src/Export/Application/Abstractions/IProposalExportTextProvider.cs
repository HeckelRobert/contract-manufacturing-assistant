namespace QuotationAccelerator.Export.Application.Abstractions;

using QuotationAccelerator.SharedKernel.Enums;

public interface IProposalExportTextProvider
{
    string Get(string key, UiLanguage language);

    string Format(string key, UiLanguage language, params object[] args);
}
