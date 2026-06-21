namespace QuotationAccelerator.Matching.Application.Abstractions;

using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.SharedKernel.Enums;

public interface IMatchReasonFormatter
{
    string Format(MatchReasonCode reasonCode, UiLanguage language);

    IReadOnlyList<string> FormatAll(
        IEnumerable<MatchReasonCode> reasonCodes,
        UiLanguage language);
}
