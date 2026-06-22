namespace QuotationAccelerator.Infrastructure.Export;

using QuotationAccelerator.Export.Domain;

internal static class ProposalExportDocumentRenderer
{
    public static IEnumerable<string> RenderHeaderLines(ProposalExportDocument document)
    {
        yield return document.ApplicationName;
        yield return document.DocumentType;

        if (!string.IsNullOrWhiteSpace(document.ProposalBasisNote))
        {
            yield return document.ProposalBasisNote;
        }

        yield return $"{document.GeneratedAtLabel}: {document.GeneratedAt:yyyy-MM-dd HH:mm}";
        yield return $"{document.MatchingStrategyLabel}: {document.MatchingStrategy}";

        if (!string.IsNullOrWhiteSpace(document.ModelNameLabel) && !string.IsNullOrWhiteSpace(document.ModelName))
        {
            yield return $"{document.ModelNameLabel}: {document.ModelName}";
        }
    }

    public static IEnumerable<string> RenderInquiryLines(ProposalExportDocument document)
    {
        yield return document.InquirySummaryTitle;

        foreach (var field in document.InquiryFields)
        {
            yield return $"{field.Label}: {field.Value}";
        }
    }

    public static IEnumerable<string> RenderMatchLines(ProposalExportDocument document)
    {
        yield return document.TopMatchesTitle;

        foreach (var match in document.Matches)
        {
            if (match.IsPrimary && !string.IsNullOrWhiteSpace(match.PrimaryIndicator))
            {
                yield return $"[{match.PrimaryIndicator}]";
            }

            yield return $"{match.ProjectNumber} — {match.ProjectName}";
            yield return $"{document.SimilarityScoreLabel}: {match.SimilarityPercent}%";
            yield return document.ReasonsLabel + ":";

            foreach (var reason in match.Reasons)
            {
                yield return $"• {reason}";
            }

            yield return string.Empty;
        }
    }

    public static IEnumerable<string> RenderProposalLines(ProposalExportDocument document)
    {
        yield return document.ManufacturingStepsTitle;
        yield return document.ManufacturingSteps;
        yield return string.Empty;
        yield return document.SuggestedQuotationTitle;
        yield return document.SuggestedQuotation;
        yield return string.Empty;
        yield return document.ReferencedDocumentsTitle;
        yield return document.ReferencedDocuments;
    }
}
