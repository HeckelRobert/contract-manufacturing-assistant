namespace QuotationAccelerator.Desktop.ViewModels;

using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuotationAccelerator.Catalog.Domain;
using QuotationAccelerator.Desktop.Resources;
using QuotationAccelerator.Desktop.Services;
using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.SharedKernel.Enums;

public partial class ProposalWorkspaceViewModel(
    IUiTextProvider uiText,
    ApplicationPreferences preferences,
    ProposalDraftBuilder proposalDraftBuilder) : ObservableObject
{
    private bool _hasLoadedProposal;

    private string _primaryProjectFolder = string.Empty;

    [ObservableProperty]
    private string _heading = string.Empty;

    [ObservableProperty]
    private string _subtitle = string.Empty;

    [ObservableProperty]
    private string _manufacturingStepsGroup = string.Empty;

    [ObservableProperty]
    private string _suggestedQuotationGroup = string.Empty;

    [ObservableProperty]
    private string _referencedDocumentsGroup = string.Empty;

    [ObservableProperty]
    private string _documentPreviewGroup = string.Empty;

    [ObservableProperty]
    private string _manufacturingSteps = string.Empty;

    [ObservableProperty]
    private string _suggestedQuotation = string.Empty;

    [ObservableProperty]
    private string _referencedDocuments = string.Empty;

    [ObservableProperty]
    private string _documentPreviewStatus = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _copyToClipboardButtonText = string.Empty;

    [ObservableProperty]
    private string _openProjectFolderButtonText = string.Empty;

    [ObservableProperty]
    private string _openDrawingButtonText = string.Empty;

    [ObservableProperty]
    private bool _canOpenDrawing;

    public void Initialize()
    {
        _hasLoadedProposal = false;
        ApplyLocalization();
    }

    public void LoadFromAnalysis(AnalyzeInquiryResult result) =>
        LoadFromMatch(result.PrimaryMatch);

    public void LoadFromMatch(ProjectMatch match)
    {
        var language = preferences.UiLanguage;
        var draft = proposalDraftBuilder.Build(match, language);

        ManufacturingSteps = draft.ManufacturingSteps;
        SuggestedQuotation = draft.SuggestedQuotation;
        ReferencedDocuments = draft.ReferencedDocuments;
        _primaryProjectFolder = draft.PrimaryProjectFolder;
        CanOpenDrawing = draft.HasDrawing;
        DocumentPreviewStatus = BuildPreviewStatus(draft, language);
        StatusMessage = uiText.Format(
            UiTextKeys.ProposalLoadedFromProjectFormat,
            language,
            match.Project.Metadata.ProjectNumber);
        _hasLoadedProposal = true;
    }

    public void ApplyLocalization()
    {
        var language = preferences.UiLanguage;
        Heading = uiText.Get(UiTextKeys.ProposalWorkspaceHeading, language);
        Subtitle = uiText.Get(UiTextKeys.ProposalWorkspaceSubtitle, language);
        ManufacturingStepsGroup = uiText.Get(UiTextKeys.ManufacturingStepsGroup, language);
        SuggestedQuotationGroup = uiText.Get(UiTextKeys.SuggestedQuotationGroup, language);
        ReferencedDocumentsGroup = uiText.Get(UiTextKeys.ReferencedDocumentsGroup, language);
        DocumentPreviewGroup = uiText.Get(UiTextKeys.DocumentPreviewGroup, language);
        CopyToClipboardButtonText = uiText.Get(UiTextKeys.CopyToClipboardButton, language);
        OpenProjectFolderButtonText = uiText.Get(UiTextKeys.OpenProjectFolderButton, language);
        OpenDrawingButtonText = uiText.Get(UiTextKeys.OpenDrawingButton, language);

        if (!_hasLoadedProposal)
        {
            ManufacturingSteps = uiText.Get(UiTextKeys.ProposalWorkspaceEmptySteps, language);
            SuggestedQuotation = uiText.Get(UiTextKeys.ProposalWorkspaceEmptyQuotation, language);
            ReferencedDocuments = uiText.Get(UiTextKeys.ProposalWorkspaceEmptyDocuments, language);
            DocumentPreviewStatus = uiText.Get(UiTextKeys.ProposalWorkspacePreviewPlaceholder, language);
            StatusMessage = string.Empty;
            CanOpenDrawing = false;
        }
    }

    [RelayCommand]
    private void CopyToClipboard()
    {
        var language = preferences.UiLanguage;
        var content = string.Join(
            Environment.NewLine + Environment.NewLine,
            uiText.Get(UiTextKeys.ManufacturingStepsGroup, language),
            ManufacturingSteps,
            uiText.Get(UiTextKeys.SuggestedQuotationGroup, language),
            SuggestedQuotation,
            uiText.Get(UiTextKeys.ReferencedDocumentsGroup, language),
            ReferencedDocuments);

        System.Windows.Clipboard.SetText(content);
        StatusMessage = uiText.Get(UiTextKeys.CopiedToClipboardStatus, language);
    }

    [RelayCommand]
    private void OpenProjectFolder()
    {
        if (!ShellLauncher.TryOpenFolder(_primaryProjectFolder, out var error))
        {
            StatusMessage = error ?? uiText.Get(UiTextKeys.OpenFolderFailed, preferences.UiLanguage);
        }
    }

    [RelayCommand]
    private void OpenDrawing()
    {
        var drawingPath = Path.Combine(_primaryProjectFolder, ProjectDocumentFileNames.Drawing);
        if (!ShellLauncher.TryOpenFile(drawingPath, out var error))
        {
            StatusMessage = error ?? uiText.Get(UiTextKeys.OpenDrawingFailed, preferences.UiLanguage);
        }
    }

    private string BuildPreviewStatus(ProposalDraft draft, UiLanguage language)
    {
        if (draft.HasDrawing)
        {
            return uiText.Format(
                UiTextKeys.ProposalPreviewDrawingAvailableFormat,
                language,
                ProjectDocumentFileNames.Drawing,
                draft.PrimaryProjectFolder);
        }

        return uiText.Format(UiTextKeys.ProposalPreviewPathFormat, language, draft.PrimaryProjectFolder);
    }
}
