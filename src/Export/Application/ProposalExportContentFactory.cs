namespace QuotationAccelerator.Export.Application;

using System.IO;
using QuotationAccelerator.Export.Application.Abstractions;
using QuotationAccelerator.Export.Application.Resources;
using QuotationAccelerator.Export.Domain;
using QuotationAccelerator.Inquiry.Domain;
using QuotationAccelerator.Matching.Application.Abstractions;
using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.SharedKernel.Enums;

public sealed class ProposalExportContentFactory(
    IMatchExplanationFormatter explanationFormatter,
    IProposalExportTextProvider text)
{
    public ProposalExportDocument Build(
        AnalyzeInquiryResult analysis,
        ProjectMatch primaryMatch,
        string manufacturingSteps,
        string suggestedQuotation,
        string referencedDocuments,
        UiLanguage language,
        string? chatModel)
    {
        var inquiry = analysis.Inquiry;
        var strategyLabel = GetMatchingStrategyLabel(analysis.Strategy, language);
        var includeModel = analysis.Strategy is MatchingStrategy.AiAssisted or MatchingStrategy.Hybrid
                           && !string.IsNullOrWhiteSpace(chatModel);

        return new ProposalExportDocument
        {
            ApplicationName = text.Get(ExportTextKeys.ApplicationName, language),
            DocumentType = text.Get(ExportTextKeys.DocumentType, language),
            GeneratedAt = DateTime.Now,
            GeneratedAtLabel = text.Get(ExportTextKeys.GeneratedAtLabel, language),
            MatchingStrategyLabel = text.Get(ExportTextKeys.MatchingStrategyLabel, language),
            MatchingStrategy = strategyLabel,
            ModelNameLabel = includeModel ? text.Get(ExportTextKeys.ModelNameLabel, language) : null,
            ModelName = includeModel ? chatModel : null,
            InquirySummaryTitle = text.Get(ExportTextKeys.InquirySummaryTitle, language),
            InquiryFields = BuildInquiryFields(inquiry, language),
            TopMatchesTitle = text.Get(ExportTextKeys.TopMatchesTitle, language),
            SimilarityScoreLabel = text.Get(ExportTextKeys.SimilarityScoreLabel, language),
            ReasonsLabel = text.Get(ExportTextKeys.ReasonsLabel, language),
            Matches = BuildMatches(analysis, primaryMatch, language),
            ManufacturingStepsTitle = text.Get(ExportTextKeys.ManufacturingStepsTitle, language),
            ManufacturingSteps = manufacturingSteps,
            SuggestedQuotationTitle = text.Get(ExportTextKeys.SuggestedQuotationTitle, language),
            SuggestedQuotation = suggestedQuotation,
            ReferencedDocumentsTitle = text.Get(ExportTextKeys.ReferencedDocumentsTitle, language),
            ReferencedDocuments = referencedDocuments,
        };
    }

    public ProposalExportDocument BuildNewContractManufacturing(
        AnalyzeInquiryResult analysis,
        string manufacturingSteps,
        string suggestedQuotation,
        string referencedDocuments,
        UiLanguage language,
        string? chatModel,
        string? referenceProjectNumber = null)
    {
        var inquiry = analysis.Inquiry;
        var strategyLabel = GetMatchingStrategyLabel(analysis.Strategy, language);
        var includeModel = analysis.Strategy is MatchingStrategy.AiAssisted or MatchingStrategy.Hybrid
                           && !string.IsNullOrWhiteSpace(chatModel);

        var basisNote = string.IsNullOrWhiteSpace(referenceProjectNumber)
            ? text.Get(ExportTextKeys.ContractManufacturingBasisNote, language)
            : text.Format(ExportTextKeys.ContractManufacturingBasisNoteWithReferenceFormat, language, referenceProjectNumber);

        return new ProposalExportDocument
        {
            ApplicationName = text.Get(ExportTextKeys.ApplicationName, language),
            DocumentType = text.Get(ExportTextKeys.ContractManufacturingDocumentType, language),
            ProposalBasisNote = basisNote,
            GeneratedAt = DateTime.Now,
            GeneratedAtLabel = text.Get(ExportTextKeys.GeneratedAtLabel, language),
            MatchingStrategyLabel = text.Get(ExportTextKeys.MatchingStrategyLabel, language),
            MatchingStrategy = strategyLabel,
            ModelNameLabel = includeModel ? text.Get(ExportTextKeys.ModelNameLabel, language) : null,
            ModelName = includeModel ? chatModel : null,
            InquirySummaryTitle = text.Get(ExportTextKeys.InquirySummaryTitle, language),
            InquiryFields = BuildInquiryFields(inquiry, language),
            TopMatchesTitle = text.Get(ExportTextKeys.TopMatchesReferenceTitle, language),
            SimilarityScoreLabel = text.Get(ExportTextKeys.SimilarityScoreLabel, language),
            ReasonsLabel = text.Get(ExportTextKeys.ReasonsLabel, language),
            Matches = BuildReferenceMatches(analysis, language),
            ManufacturingStepsTitle = text.Get(ExportTextKeys.ManufacturingStepsTitle, language),
            ManufacturingSteps = manufacturingSteps,
            SuggestedQuotationTitle = text.Get(ExportTextKeys.SuggestedQuotationTitle, language),
            SuggestedQuotation = suggestedQuotation,
            ReferencedDocumentsTitle = text.Get(ExportTextKeys.ReferencedDocumentsTitle, language),
            ReferencedDocuments = referencedDocuments,
        };
    }

    private IReadOnlyList<ProposalExportMatchEntry> BuildReferenceMatches(
        AnalyzeInquiryResult analysis,
        UiLanguage language) =>
        analysis.Matches
            .Select(match =>
            {
                var metadata = match.Project.Metadata;
                return new ProposalExportMatchEntry
                {
                    ProjectNumber = metadata.ProjectNumber,
                    ProjectName = metadata.Title,
                    SimilarityPercent = match.SimilarityPercent,
                    Reasons = explanationFormatter.FormatExplanations(analysis.Inquiry, match, language),
                    IsPrimary = false,
                    PrimaryIndicator = null,
                };
            })
            .ToList();

    private IReadOnlyList<ProposalExportField> BuildInquiryFields(CustomerInquiry inquiry, UiLanguage language)
    {
        var notProvided = text.Get(ExportTextKeys.NotProvided, language);

        return
        [
            Field(ExportTextKeys.PartDescriptionLabel, inquiry.PartDescription, language, notProvided),
            Field(ExportTextKeys.QuantityLabel, inquiry.Quantity?.ToString(), language, notProvided),
            Field(ExportTextKeys.MaterialLabel, inquiry.Material, language, notProvided),
            Field(ExportTextKeys.SurfaceTreatmentLabel, inquiry.SurfaceTreatment, language, notProvided),
            Field(ExportTextKeys.DeliveryDeadlineLabel, inquiry.DeliveryDeadline, language, notProvided),
            Field(ExportTextKeys.SpecialRequirementsLabel, inquiry.SpecialRequirements, language, notProvided),
            Field(
                ExportTextKeys.DrawingFilenameLabel,
                inquiry.HasDrawing ? Path.GetFileName(inquiry.DrawingFilePath!) : null,
                language,
                notProvided),
        ];
    }

    private IReadOnlyList<ProposalExportMatchEntry> BuildMatches(
        AnalyzeInquiryResult analysis,
        ProjectMatch primaryMatch,
        UiLanguage language)
    {
        var primaryIndicator = text.Get(ExportTextKeys.PrimaryMatchIndicator, language);

        return analysis.Matches
            .Select(match =>
            {
                var metadata = match.Project.Metadata;
                var isPrimary = ReferenceEquals(match, primaryMatch)
                                || string.Equals(
                                    metadata.ProjectNumber,
                                    primaryMatch.Project.Metadata.ProjectNumber,
                                    StringComparison.OrdinalIgnoreCase);

                return new ProposalExportMatchEntry
                {
                    ProjectNumber = metadata.ProjectNumber,
                    ProjectName = metadata.Title,
                    SimilarityPercent = match.SimilarityPercent,
                    Reasons = explanationFormatter.FormatExplanations(analysis.Inquiry, match, language),
                    IsPrimary = isPrimary,
                    PrimaryIndicator = isPrimary ? primaryIndicator : null,
                };
            })
            .ToList();
    }

    private ProposalExportField Field(
        string labelKey,
        string? value,
        UiLanguage language,
        string notProvided) =>
        new()
        {
            Label = text.Get(labelKey, language),
            Value = string.IsNullOrWhiteSpace(value) ? notProvided : value,
        };

    private string GetMatchingStrategyLabel(MatchingStrategy strategy, UiLanguage language) =>
        strategy switch
        {
            MatchingStrategy.RuleBased => text.Get(ExportTextKeys.MatchingStrategyRuleBased, language),
            MatchingStrategy.AiAssisted => text.Get(ExportTextKeys.MatchingStrategyAiAssisted, language),
            MatchingStrategy.Hybrid => text.Get(ExportTextKeys.MatchingStrategyHybrid, language),
            _ => strategy.ToString(),
        };
}
