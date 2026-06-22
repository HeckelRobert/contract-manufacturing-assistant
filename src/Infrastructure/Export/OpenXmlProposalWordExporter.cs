namespace QuotationAccelerator.Infrastructure.Export;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using QuotationAccelerator.Export.Application.Abstractions;
using QuotationAccelerator.Export.Domain;
using QuotationAccelerator.SharedKernel.Results;

public sealed class OpenXmlProposalWordExporter : IProposalWordExporter
{
    public Result Export(ProposalExportDocument document, string filePath)
    {
        try
        {
            using var wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document);
            var mainPart = wordDocument.AddMainDocumentPart();
            mainPart.Document = new Document(new Body());
            var body = mainPart.Document.Body!;

            foreach (var line in ProposalExportDocumentRenderer.RenderHeaderLines(document))
            {
                body.AppendChild(CreateParagraph(line, bold: line == document.ApplicationName));
            }

            body.AppendChild(CreateParagraph(string.Empty));

            foreach (var line in ProposalExportDocumentRenderer.RenderInquiryLines(document))
            {
                body.AppendChild(CreateParagraph(line, bold: line == document.InquirySummaryTitle));
            }

            body.AppendChild(CreateParagraph(string.Empty));

            body.AppendChild(CreateParagraph(document.TopMatchesTitle, bold: true));

            foreach (var match in document.Matches)
            {
                if (match.IsPrimary && !string.IsNullOrWhiteSpace(match.PrimaryIndicator))
                {
                    body.AppendChild(CreateParagraph($"[{match.PrimaryIndicator}]", bold: true));
                }

                body.AppendChild(CreateParagraph(
                    $"{match.ProjectNumber} — {match.ProjectName}",
                    bold: match.IsPrimary));
                body.AppendChild(CreateParagraph($"{document.SimilarityScoreLabel}: {match.SimilarityPercent}%"));
                body.AppendChild(CreateParagraph(document.ReasonsLabel + ":", bold: true));

                foreach (var reason in match.Reasons)
                {
                    body.AppendChild(CreateParagraph($"• {reason}"));
                }

                body.AppendChild(CreateParagraph(string.Empty));
            }

            body.AppendChild(CreateParagraph(document.ManufacturingStepsTitle, bold: true));
            AppendMultilineParagraphs(body, document.ManufacturingSteps);
            body.AppendChild(CreateParagraph(string.Empty));

            body.AppendChild(CreateParagraph(document.SuggestedQuotationTitle, bold: true));
            AppendMultilineParagraphs(body, document.SuggestedQuotation);
            body.AppendChild(CreateParagraph(string.Empty));

            body.AppendChild(CreateParagraph(document.ReferencedDocumentsTitle, bold: true));
            AppendMultilineParagraphs(body, document.ReferencedDocuments);

            mainPart.Document.Save();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    private static void AppendMultilineParagraphs(Body body, string content)
    {
        var lines = content.Replace("\r\n", "\n").Split('\n');
        foreach (var line in lines)
        {
            body.AppendChild(CreateParagraph(line));
        }
    }

    private static Paragraph CreateParagraph(string text, bool bold = false)
    {
        var run = new Run(new Text(text) { Space = SpaceProcessingModeValues.Preserve });
        if (bold)
        {
            run.RunProperties = new RunProperties(new Bold());
        }

        return new Paragraph(run);
    }
}
