namespace QuotationAccelerator.Matching.Application.AnalyzeInquiry;

using FluentValidation;
using QuotationAccelerator.Inquiry.Domain;

public sealed class AnalyzeInquiryValidator : AbstractValidator<AnalyzeInquiryCommand>
{
    public AnalyzeInquiryValidator()
    {
        RuleFor(x => x.Inquiry.Material)
            .NotEmpty()
            .WithMessage(AnalyzeInquiryValidationKeys.MaterialRequired);

        RuleFor(x => x.Inquiry)
            .Must(inquiry => inquiry.HasDrawing || inquiry.HasPartDescription)
            .WithMessage(AnalyzeInquiryValidationKeys.DrawingOrDescriptionRequired);
    }
}
