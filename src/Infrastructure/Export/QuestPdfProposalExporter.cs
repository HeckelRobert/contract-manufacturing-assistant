namespace QuotationAccelerator.Infrastructure.Export;

using QuotationAccelerator.Export.Application.Abstractions;
using QuotationAccelerator.Export.Domain;
using QuotationAccelerator.SharedKernel.Results;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public sealed class QuestPdfProposalExporter : IProposalPdfExporter
{
    static QuestPdfProposalExporter()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Result Export(ProposalExportDocument document, string filePath)
    {
        try
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(style => style.FontSize(11));

                    page.Content().Column(column =>
                    {
                        column.Spacing(8);
                        RenderHeader(column, document);
                        column.Item().PaddingTop(8);
                        RenderSection(column, document.InquirySummaryTitle, ProposalExportDocumentRenderer.RenderInquiryLines(document).Skip(1));
                        column.Item().PaddingTop(8);
                        RenderMatches(column, document);
                        column.Item().PaddingTop(8);
                        RenderTextSection(column, document.ManufacturingStepsTitle, document.ManufacturingSteps);
                        column.Item().PaddingTop(8);
                        RenderTextSection(column, document.SuggestedQuotationTitle, document.SuggestedQuotation);
                        column.Item().PaddingTop(8);
                        RenderTextSection(column, document.ReferencedDocumentsTitle, document.ReferencedDocuments);
                    });
                });
            }).GeneratePdf(filePath);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    private static void RenderHeader(ColumnDescriptor column, ProposalExportDocument document)
    {
        column.Item().Text(document.ApplicationName).Bold().FontSize(16);
        column.Item().Text(document.DocumentType).SemiBold().FontSize(13);

        if (!string.IsNullOrWhiteSpace(document.ProposalBasisNote))
        {
            column.Item().Text(document.ProposalBasisNote).Italic();
        }

        column.Item().Text($"{document.GeneratedAtLabel}: {document.GeneratedAt:yyyy-MM-dd HH:mm}");
        column.Item().Text($"{document.MatchingStrategyLabel}: {document.MatchingStrategy}");

        if (!string.IsNullOrWhiteSpace(document.ModelNameLabel) && !string.IsNullOrWhiteSpace(document.ModelName))
        {
            column.Item().Text($"{document.ModelNameLabel}: {document.ModelName}");
        }
    }

    private static void RenderSection(ColumnDescriptor column, string title, IEnumerable<string> lines)
    {
        column.Item().Text(title).Bold().FontSize(13);
        foreach (var line in lines)
        {
            column.Item().Text(line);
        }
    }

    private static void RenderMatches(ColumnDescriptor column, ProposalExportDocument document)
    {
        column.Item().Text(document.TopMatchesTitle).Bold().FontSize(13);

        foreach (var match in document.Matches)
        {
            if (match.IsPrimary && !string.IsNullOrWhiteSpace(match.PrimaryIndicator))
            {
                column.Item().Text($"[{match.PrimaryIndicator}]").SemiBold().FontColor(Colors.Blue.Darken2);
            }

            var titleStyle = match.IsPrimary
                ? TextStyle.Default.SemiBold()
                : TextStyle.Default;

            column.Item().Text($"{match.ProjectNumber} — {match.ProjectName}").Style(titleStyle);
            column.Item().Text($"{document.SimilarityScoreLabel}: {match.SimilarityPercent}%");
            column.Item().Text(document.ReasonsLabel + ":");

            foreach (var reason in match.Reasons)
            {
                column.Item().Text($"• {reason}");
            }

            column.Item().PaddingBottom(6);
        }
    }

    private static void RenderTextSection(ColumnDescriptor column, string title, string content)
    {
        column.Item().Text(title).Bold().FontSize(13);
        column.Item().Text(content).LineHeight(1.3f);
    }
}
