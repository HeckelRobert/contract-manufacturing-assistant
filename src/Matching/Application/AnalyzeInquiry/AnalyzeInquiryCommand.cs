namespace QuotationAccelerator.Matching.Application.AnalyzeInquiry;

using QuotationAccelerator.Inquiry.Domain;
using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Enums;
using QuotationAccelerator.SharedKernel.Results;

public sealed record AnalyzeInquiryCommand(
    CustomerInquiry Inquiry,
    MatchingStrategy Strategy) : ICommand<Result<AnalyzeInquiryResult>>;
