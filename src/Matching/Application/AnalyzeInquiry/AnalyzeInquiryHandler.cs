namespace QuotationAccelerator.Matching.Application.AnalyzeInquiry;

using FluentValidation;
using Microsoft.Extensions.Logging;
using QuotationAccelerator.Matching.Application.Abstractions;
using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Enums;
using QuotationAccelerator.SharedKernel.Results;

public sealed class AnalyzeInquiryHandler(
    IMatchingStrategyResolver strategyResolver,
    IValidator<AnalyzeInquiryCommand> validator,
    ILogger<AnalyzeInquiryHandler> logger) : ICommandHandler<AnalyzeInquiryCommand, Result<AnalyzeInquiryResult>>
{
    public async Task<Result<AnalyzeInquiryResult>> HandleAsync(
        AnalyzeInquiryCommand command,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<AnalyzeInquiryResult>.Failure(
                validation.Errors.Select(error => error.ErrorMessage));
        }

        if (command.Strategy == MatchingStrategy.AiAssisted)
        {
            return Result<AnalyzeInquiryResult>.Failure(AnalyzeInquiryErrorCodes.AiAssistedNotAvailable);
        }

        try
        {
            var strategy = strategyResolver.Resolve(command.Strategy);
            var matches = await strategy.MatchAsync(command.Inquiry, cancellationToken);
            if (matches.Count == 0)
            {
                return Result<AnalyzeInquiryResult>.Failure(AnalyzeInquiryErrorCodes.NoSimilarProjectsFound);
            }

            logger.LogInformation(
                "Analyzed inquiry using {Strategy} and found {MatchCount} matches",
                command.Strategy,
                matches.Count);

            return Result<AnalyzeInquiryResult>.Success(new AnalyzeInquiryResult
            {
                Inquiry = command.Inquiry,
                Strategy = command.Strategy,
                Matches = matches,
            });
        }
        catch (NotSupportedException ex)
        {
            return Result<AnalyzeInquiryResult>.Failure(ex.Message);
        }
    }
}
