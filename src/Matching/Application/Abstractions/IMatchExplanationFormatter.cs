namespace QuotationAccelerator.Matching.Application.Abstractions;

using QuotationAccelerator.Inquiry.Domain;
using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.SharedKernel.Enums;

public interface IMatchExplanationFormatter
{
    IReadOnlyList<string> FormatExplanations(
        CustomerInquiry inquiry,
        ProjectMatch match,
        UiLanguage language);
}
