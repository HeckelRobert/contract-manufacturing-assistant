namespace QuotationAccelerator.Inbox.Application.ContinueContractManufacturingInquiry;

using FluentValidation;
using Microsoft.Extensions.Options;
using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.Inquiry.Domain;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Configuration;
using QuotationAccelerator.SharedKernel.Results;

public sealed record ContinueContractManufacturingInquiryCommand(string InboxMessageId)
    : ICommand<Result<ContinueContractManufacturingInquiryResult>>;

public sealed class ContinueContractManufacturingInquiryResult
{
    public required CustomerInquiry Inquiry { get; init; }

    public required string InboxMessageId { get; init; }

    public required string GraphMessageId { get; init; }

    public required string ReplyToAddress { get; init; }

    public required string OriginalSubject { get; init; }

    public string? WorkPreparationSummary { get; init; }
}

public sealed class ContinueContractManufacturingInquiryValidator
    : AbstractValidator<ContinueContractManufacturingInquiryCommand>
{
    public ContinueContractManufacturingInquiryValidator()
    {
        RuleFor(command => command.InboxMessageId).NotEmpty();
    }
}

public sealed class ContinueContractManufacturingInquiryHandler(
    IInboxMessageRepository messageRepository,
    IInquiryExtractionService extractionService,
    IOptions<InquiryOptions> inquiryOptions,
    IValidator<ContinueContractManufacturingInquiryCommand> validator)
    : ICommandHandler<ContinueContractManufacturingInquiryCommand, Result<ContinueContractManufacturingInquiryResult>>
{
    public async Task<Result<ContinueContractManufacturingInquiryResult>> HandleAsync(
        ContinueContractManufacturingInquiryCommand command,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<ContinueContractManufacturingInquiryResult>.Failure(
                validation.Errors.Select(error => error.ErrorMessage));
        }

        var message = await messageRepository.GetByIdAsync(command.InboxMessageId, cancellationToken);
        if (message is null)
        {
            return Result<ContinueContractManufacturingInquiryResult>.Failure("Inbox message was not found.");
        }

        if (message.Category != InboxMessageCategory.ContractManufacturingInquiry)
        {
            return Result<ContinueContractManufacturingInquiryResult>.Failure(
                "Only contract-manufacturing inquiry messages can be continued.");
        }

        var extracted = extractionService.Extract(message.Subject, message.BodyText ?? message.BodyPreview);
        var drawingPath = message.Attachments
            .FirstOrDefault(attachment => attachment.IsPdf && !string.IsNullOrWhiteSpace(attachment.LocalPath))
            ?.LocalPath;

        var nonPdfAttachments = message.Attachments
            .Where(attachment => !attachment.IsPdf)
            .Select(attachment => attachment.FileName)
            .ToList();

        var notes = BuildNotes(message, nonPdfAttachments, extracted.WorkPreparationSummary);
        var options = inquiryOptions.Value;

        var inquiry = extracted.ToCustomerInquiry(
            drawingPath,
            notes,
            options.Materials,
            options.SurfaceTreatments,
            options.ManufacturingProcesses);

        await messageRepository.UpdateStatusAsync(
            message.Id,
            InboxMessageStatus.ContinuedToInquiry,
            cancellationToken);

        return Result<ContinueContractManufacturingInquiryResult>.Success(
            new ContinueContractManufacturingInquiryResult
            {
                Inquiry = inquiry,
                InboxMessageId = message.Id,
                GraphMessageId = message.GraphMessageId,
                ReplyToAddress = message.FromAddress,
                OriginalSubject = message.Subject,
                WorkPreparationSummary = extracted.WorkPreparationSummary,
            });
    }

    private static string BuildNotes(
        InboxMessage message,
        IReadOnlyList<string> nonPdfAttachments,
        string? workPreparationSummary)
    {
        var lines = new List<string>
        {
            $"Source email: {message.Subject}",
            $"From: {message.FromDisplayName ?? message.FromAddress} <{message.FromAddress}>",
        };

        if (nonPdfAttachments.Count > 0)
        {
            lines.Add($"Additional attachments (reference only): {string.Join(", ", nonPdfAttachments)}");
        }

        if (!string.IsNullOrWhiteSpace(workPreparationSummary))
        {
            lines.Add("Work preparation (extracted):");
            lines.Add(workPreparationSummary);
        }

        return string.Join(Environment.NewLine, lines);
    }
}
