namespace QuotationAccelerator.Desktop.Services;

using System.Text;
using QuotationAccelerator.Catalog.Domain;
using QuotationAccelerator.Desktop.Resources;
using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.SharedKernel.Enums;

public sealed class ProposalDraftBuilder(IUiTextProvider uiText)
{
    public ProposalDraft Build(ProjectMatch match, UiLanguage language)
    {
        var primary = match.Project;
        var metadata = primary.Metadata;

        var steps = new StringBuilder();
        foreach (var process in metadata.Processes)
        {
            steps.AppendLine(uiText.Format(UiTextKeys.ProposalStepFormat, language, process));
        }

        if (primary.DocumentFileNames.Any(name =>
                name.Equals(ProjectDocumentFileNames.Fixture, StringComparison.OrdinalIgnoreCase)))
        {
            steps.AppendLine(uiText.Get(UiTextKeys.ProposalFixtureAvailable, language));
        }

        if (primary.DocumentFileNames.Any(name =>
                name.Equals(ProjectDocumentFileNames.CncProgram, StringComparison.OrdinalIgnoreCase)))
        {
            steps.AppendLine(uiText.Get(UiTextKeys.ProposalCncProgramAvailable, language));
        }

        if (steps.Length == 0)
        {
            steps.Append(uiText.Get(UiTextKeys.ProposalNoStepsFromMatch, language));
        }

        var quotation = uiText.Format(
            UiTextKeys.ProposalQuotationDraftFormat,
            language,
            metadata.ProjectNumber,
            metadata.Title,
            metadata.Material,
            metadata.Quantity,
            metadata.SurfaceTreatment ?? uiText.Get(UiTextKeys.SurfaceTreatment_None, language),
            match.SimilarityPercent);

        var documents = new StringBuilder();
        if (primary.DocumentFileNames.Count == 0)
        {
            documents.Append(uiText.Get(UiTextKeys.ProposalNoDocumentsListed, language));
        }
        else
        {
            foreach (var document in primary.DocumentFileNames)
            {
                documents.AppendLine(uiText.Format(UiTextKeys.ProposalDocumentLineFormat, language, document));
            }
        }

        var hasDrawing = primary.DocumentFileNames.Any(name =>
            name.Equals(ProjectDocumentFileNames.Drawing, StringComparison.OrdinalIgnoreCase));

        return new ProposalDraft
        {
            ManufacturingSteps = steps.ToString().TrimEnd(),
            SuggestedQuotation = quotation,
            ReferencedDocuments = documents.ToString().TrimEnd(),
            PrimaryProjectFolder = primary.FolderPath,
            HasDrawing = hasDrawing,
        };
    }

    public ProposalDraft Build(AnalyzeInquiryResult result, UiLanguage language) =>
        Build(result.PrimaryMatch, language);
}

public sealed class ProposalDraft
{
    public required string ManufacturingSteps { get; init; }

    public required string SuggestedQuotation { get; init; }

    public required string ReferencedDocuments { get; init; }

    public required string PrimaryProjectFolder { get; init; }

    public bool HasDrawing { get; init; }
}
