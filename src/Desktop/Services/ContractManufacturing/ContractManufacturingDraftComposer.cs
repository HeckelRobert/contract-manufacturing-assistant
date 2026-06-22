namespace QuotationAccelerator.Desktop.Services.ContractManufacturing;

using System.IO;
using System.Text;
using QuotationAccelerator.Catalog.Domain;
using QuotationAccelerator.Desktop.Resources;
using QuotationAccelerator.Desktop.Services;
using QuotationAccelerator.Inquiry.Domain;
using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.SharedKernel.Enums;

public sealed class ContractManufacturingDraftComposer(
    ContractManufacturingTemplateProvider templateProvider,
    IUiTextProvider uiText)
{
    public ProposalDraft Compose(
        CustomerInquiry inquiry,
        UiLanguage language,
        ProjectMatch? referenceProject)
    {
        var template = templateProvider.GetTemplate(language);
        var tokens = BuildTokens(template, inquiry, referenceProject, language);

        var manufacturingSteps = ComposeManufacturingSteps(template, tokens, inquiry, referenceProject, language);
        var quotation = ReplaceTokens(template.Quotation, tokens).TrimEnd();
        var referencedDocuments = ReplaceTokens(template.ReferencedDocuments, tokens).TrimEnd();

        return new ProposalDraft
        {
            ManufacturingSteps = manufacturingSteps,
            SuggestedQuotation = quotation,
            ReferencedDocuments = referencedDocuments,
            PrimaryProjectFolder = string.Empty,
            HasDrawing = inquiry.HasDrawing && File.Exists(inquiry.DrawingFilePath),
            CustomerDrawingPath = inquiry.DrawingFilePath,
            SourceMode = ProposalSourceMode.NewContractManufacturing,
            ReferenceProjectNumber = referenceProject?.Project.Metadata.ProjectNumber,
        };
    }

    private string ComposeManufacturingSteps(
        ContractManufacturingTemplate template,
        IReadOnlyDictionary<string, string> tokens,
        CustomerInquiry inquiry,
        ProjectMatch? referenceProject,
        UiLanguage language)
    {
        var steps = new StringBuilder();

        foreach (var line in template.ManufacturingSteps)
        {
            if (line == "{InquiryProcesses}")
            {
                AppendInquiryProcesses(steps, template, inquiry, referenceProject, language);
                continue;
            }

            if (line == "{WorkPreparationSummary}")
            {
                AppendWorkPreparationSummary(steps, inquiry);
                continue;
            }

            steps.AppendLine(ReplaceTokens(line, tokens));
        }

        if (referenceProject is not null)
        {
            steps.AppendLine();
            steps.AppendLine(ReplaceTokens(template.ReferenceManufacturingStepsIntro, tokens));

            foreach (var process in referenceProject.Project.Metadata.Processes)
            {
                steps.AppendLine(uiText.Format(UiTextKeys.ProposalStepFormat, language, process));
            }

            if (referenceProject.Project.DocumentFileNames.Any(name =>
                    name.Equals(ProjectDocumentFileNames.Fixture, StringComparison.OrdinalIgnoreCase)))
            {
                steps.AppendLine(uiText.Get(UiTextKeys.ProposalFixtureAvailable, language));
            }

            if (referenceProject.Project.DocumentFileNames.Any(name =>
                    name.Equals(ProjectDocumentFileNames.CncProgram, StringComparison.OrdinalIgnoreCase)))
            {
                steps.AppendLine(uiText.Get(UiTextKeys.ProposalCncProgramAvailable, language));
            }
        }

        return steps.ToString().TrimEnd();
    }

    private static void AppendWorkPreparationSummary(StringBuilder steps, CustomerInquiry inquiry)
    {
        if (string.IsNullOrWhiteSpace(inquiry.Notes))
        {
            return;
        }

        const string marker = "Work preparation (extracted):";
        var markerIndex = inquiry.Notes.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (markerIndex < 0)
        {
            return;
        }

        var summary = inquiry.Notes[(markerIndex + marker.Length)..].Trim();
        if (string.IsNullOrWhiteSpace(summary))
        {
            return;
        }

        foreach (var line in summary.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
        {
            steps.AppendLine($"• {line.Trim()}");
        }
    }

    private void AppendInquiryProcesses(
        StringBuilder steps,
        ContractManufacturingTemplate template,
        CustomerInquiry inquiry,
        ProjectMatch? referenceProject,
        UiLanguage language)
    {
        var processes = inquiry.ManufacturingProcesses.Count > 0
            ? inquiry.ManufacturingProcesses
            : referenceProject?.Project.Metadata.Processes ?? [];

        if (processes.Count == 0)
        {
            steps.AppendLine(uiText.Get(UiTextKeys.ContractManufacturingStepProcessPlanning, language));
            return;
        }

        foreach (var process in processes)
        {
            steps.AppendLine(
                ReplaceTokens(template.InquiryProcessLine, new Dictionary<string, string>
                {
                    ["ProcessName"] = process,
                }));
        }
    }

    private Dictionary<string, string> BuildTokens(
        ContractManufacturingTemplate template,
        CustomerInquiry inquiry,
        ProjectMatch? referenceProject,
        UiLanguage language)
    {
        var notProvided = uiText.Get(UiTextKeys.NotProvided, language);
        var surface = inquiry.SurfaceTreatment ?? uiText.Get(UiTextKeys.SurfaceTreatment_None, language);
        var hasReference = referenceProject is not null;
        var referenceMetadata = referenceProject?.Project.Metadata;

        var customerDrawingLine = inquiry.HasDrawing
            ? ReplaceTokens(
                template.CustomerDrawingLine,
                new Dictionary<string, string>
                {
                    ["DrawingFilename"] = Path.GetFileName(inquiry.DrawingFilePath!),
                })
            : template.NoCustomerDrawingLine;

        var referenceDocumentList = hasReference
            ? string.Join(
                Environment.NewLine,
                referenceProject!.Project.DocumentFileNames.Select(document =>
                    uiText.Format(UiTextKeys.ProposalDocumentLineFormat, language, document)))
            : string.Empty;

        var referenceTokens = hasReference
            ? new Dictionary<string, string>
            {
                ["ReferenceProjectNumber"] = referenceMetadata!.ProjectNumber,
                ["ReferenceTitle"] = referenceMetadata.Title,
                ["ReferenceMaterial"] = referenceMetadata.Material,
                ["ReferenceQuantity"] = referenceMetadata.Quantity.ToString(),
                ["ReferenceSurfaceTreatment"] = referenceMetadata.SurfaceTreatment ?? notProvided,
                ["ReferenceDocumentList"] = referenceDocumentList,
            }
            : new Dictionary<string, string>();

        return new Dictionary<string, string>
        {
            ["PartDescription"] = string.IsNullOrWhiteSpace(inquiry.PartDescription) ? notProvided : inquiry.PartDescription,
            ["Material"] = inquiry.Material,
            ["Quantity"] = (inquiry.Quantity ?? 1).ToString(),
            ["SurfaceTreatment"] = surface,
            ["DeliveryDeadline"] = string.IsNullOrWhiteSpace(inquiry.DeliveryDeadline) ? notProvided : inquiry.DeliveryDeadline,
            ["SpecialRequirements"] = string.IsNullOrWhiteSpace(inquiry.SpecialRequirements) ? notProvided : inquiry.SpecialRequirements,
            ["DrawingFilename"] = inquiry.HasDrawing ? Path.GetFileName(inquiry.DrawingFilePath!) : notProvided,
            ["CustomerDrawingLine"] = customerDrawingLine,
            ["ReferenceProjectNumber"] = referenceMetadata?.ProjectNumber ?? string.Empty,
            ["ReferenceTitle"] = referenceMetadata?.Title ?? string.Empty,
            ["ReferenceMaterial"] = referenceMetadata?.Material ?? string.Empty,
            ["ReferenceQuantity"] = referenceMetadata?.Quantity.ToString() ?? string.Empty,
            ["ReferenceSurfaceTreatment"] = referenceMetadata?.SurfaceTreatment ?? notProvided,
            ["ReferenceDocumentList"] = referenceDocumentList,
            ["ReferenceQuotationBlock"] = hasReference
                ? ReplaceTokens(template.ReferenceQuotationBlock, referenceTokens)
                : string.Empty,
            ["ReferenceDocumentsBlock"] = hasReference
                ? ReplaceTokens(template.ReferenceDocumentsBlock, referenceTokens)
                : string.Empty,
        };
    }

    private static string ReplaceTokens(string text, IReadOnlyDictionary<string, string> tokens)
    {
        var result = text;
        foreach (var (key, value) in tokens)
        {
            result = result.Replace($"{{{key}}}", value, StringComparison.Ordinal);
        }

        return result;
    }
}
