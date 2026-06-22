namespace QuotationAccelerator.Export.Domain;

public sealed class ProposalExportDocument
{
    public required string ApplicationName { get; init; }

    public required string DocumentType { get; init; }

    public string? ProposalBasisNote { get; init; }

    public required DateTime GeneratedAt { get; init; }

    public required string GeneratedAtLabel { get; init; }

    public required string MatchingStrategyLabel { get; init; }

    public required string MatchingStrategy { get; init; }

    public string? ModelNameLabel { get; init; }

    public string? ModelName { get; init; }

    public required string InquirySummaryTitle { get; init; }

    public required IReadOnlyList<ProposalExportField> InquiryFields { get; init; }

    public required string TopMatchesTitle { get; init; }

    public required string SimilarityScoreLabel { get; init; }

    public required string ReasonsLabel { get; init; }

    public required IReadOnlyList<ProposalExportMatchEntry> Matches { get; init; }

    public required string ManufacturingStepsTitle { get; init; }

    public required string ManufacturingSteps { get; init; }

    public required string SuggestedQuotationTitle { get; init; }

    public required string SuggestedQuotation { get; init; }

    public required string ReferencedDocumentsTitle { get; init; }

    public required string ReferencedDocuments { get; init; }
}

public sealed class ProposalExportField
{
    public required string Label { get; init; }

    public required string Value { get; init; }
}

public sealed class ProposalExportMatchEntry
{
    public required string ProjectNumber { get; init; }

    public required string ProjectName { get; init; }

    public required int SimilarityPercent { get; init; }

    public required IReadOnlyList<string> Reasons { get; init; }

    public bool IsPrimary { get; init; }

    public string? PrimaryIndicator { get; init; }
}
