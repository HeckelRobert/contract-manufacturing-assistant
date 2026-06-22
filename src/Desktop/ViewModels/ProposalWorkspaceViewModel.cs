namespace QuotationAccelerator.Desktop.ViewModels;

using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using QuotationAccelerator.Catalog.Domain;
using QuotationAccelerator.Desktop.Resources;
using QuotationAccelerator.Desktop.Services;
using QuotationAccelerator.Export.Application;
using QuotationAccelerator.Export.Application.Abstractions;
using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.Inbox.Application.SendProposalReply;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Configuration;
using QuotationAccelerator.SharedKernel.Enums;

public partial class ProposalWorkspaceViewModel(
    IUiTextProvider uiText,
    ApplicationPreferences preferences,
    ProposalDraftBuilder proposalDraftBuilder,
    ProposalExportContentFactory exportContentFactory,
    IProposalPdfExporter pdfExporter,
    IProposalWordExporter wordExporter,
    IOptions<AiOptions> aiOptions,
    IDispatcher dispatcher,
    InboxSessionContext inboxSession) : ObservableObject
{
    private bool _hasLoadedProposal;
    private AnalyzeInquiryResult? _analysisResult;
    private ProjectMatch? _primaryMatch;
    private ProposalSourceMode _sourceMode = ProposalSourceMode.HistoricalProject;
    private string? _customerDrawingPath;
    private string? _referenceProjectNumber;

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
    private string? _previewPdfPath;

    [ObservableProperty]
    private string _previewPlaceholderMessage = string.Empty;

    [ObservableProperty]
    private string _pdfPreviewRuntimeMissingMessage = string.Empty;

    [ObservableProperty]
    private string _exportToPdfButtonText = string.Empty;

    [ObservableProperty]
    private string _exportMenuButtonText = string.Empty;

    [ObservableProperty]
    private string _exportToWordButtonText = string.Empty;

    [ObservableProperty]
    private string _sendAsEmailButtonText = string.Empty;

    [ObservableProperty]
    private bool _canSendAsEmail;

    [ObservableProperty]
    private bool _canExport;

    [ObservableProperty]
    private bool _canOpenDrawing;

    [ObservableProperty]
    private bool _canOpenProjectFolder;

    [ObservableProperty]
    private bool _canCreateNewContractManufacturing;

    [ObservableProperty]
    private string _createNewContractManufacturingButtonText = string.Empty;

    public void Initialize()
    {
        _hasLoadedProposal = false;
        ApplyLocalization();
    }

    public void LoadFromAnalysis(AnalyzeInquiryResult result)
    {
        _analysisResult = result;
        LoadFromMatch(result.PrimaryMatch);
        UpdateCreateNewContractManufacturingState();
    }

    public void LoadNewContractManufacturing(NewContractManufacturingRequest request)
    {
        _analysisResult = request.Analysis;
        _primaryMatch = null;
        _sourceMode = ProposalSourceMode.NewContractManufacturing;

        var language = preferences.UiLanguage;
        var draft = proposalDraftBuilder.BuildFromInquiry(
            request.Analysis.Inquiry,
            language,
            request.ReferenceProject);

        ManufacturingSteps = draft.ManufacturingSteps;
        SuggestedQuotation = draft.SuggestedQuotation;
        ReferencedDocuments = draft.ReferencedDocuments;
        _primaryProjectFolder = draft.PrimaryProjectFolder;
        _customerDrawingPath = draft.CustomerDrawingPath;
        _referenceProjectNumber = draft.ReferenceProjectNumber;
        CanOpenDrawing = draft.HasDrawing;
        CanOpenProjectFolder = false;
        Heading = uiText.Get(UiTextKeys.ContractManufacturingWorkspaceHeading, language);
        Subtitle = request.ReferenceProject is not null
            ? uiText.Format(
                UiTextKeys.ContractManufacturingWorkspaceSubtitleWithReferenceFormat,
                language,
                request.ReferenceProject.Project.Metadata.ProjectNumber)
            : uiText.Get(UiTextKeys.ContractManufacturingWorkspaceSubtitle, language);
        UpdateDrawingPreview(draft.HasDrawing);
        StatusMessage = request.ReferenceProject is not null
            ? uiText.Format(
                UiTextKeys.ProposalLoadedAsContractManufacturingWithReferenceFormat,
                language,
                request.ReferenceProject.Project.Metadata.ProjectNumber)
            : uiText.Get(UiTextKeys.ProposalLoadedAsContractManufacturingStatus, language);
        _hasLoadedProposal = true;
        UpdateExportState();
        UpdateCreateNewContractManufacturingState();
    }

    public void LoadFromMatch(ProjectMatch match)
    {
        _primaryMatch = match;
        _sourceMode = ProposalSourceMode.HistoricalProject;
        _customerDrawingPath = null;
        _referenceProjectNumber = null;
        var language = preferences.UiLanguage;
        var draft = proposalDraftBuilder.Build(match, language);

        ManufacturingSteps = draft.ManufacturingSteps;
        SuggestedQuotation = draft.SuggestedQuotation;
        ReferencedDocuments = draft.ReferencedDocuments;
        _primaryProjectFolder = draft.PrimaryProjectFolder;
        CanOpenDrawing = draft.HasDrawing;
        CanOpenProjectFolder = !string.IsNullOrWhiteSpace(draft.PrimaryProjectFolder);
        Heading = uiText.Get(UiTextKeys.ProposalWorkspaceHeading, language);
        Subtitle = uiText.Get(UiTextKeys.ProposalWorkspaceSubtitle, language);
        UpdateDrawingPreview(draft.HasDrawing);
        StatusMessage = uiText.Format(
            UiTextKeys.ProposalLoadedFromProjectFormat,
            language,
            match.Project.Metadata.ProjectNumber);
        _hasLoadedProposal = true;
        UpdateExportState();
        UpdateCreateNewContractManufacturingState();
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
        ExportToPdfButtonText = uiText.Get(UiTextKeys.ExportToPdfButton, language);
        ExportMenuButtonText = uiText.Get(UiTextKeys.ExportMenuButton, language);
        ExportToWordButtonText = uiText.Get(UiTextKeys.ExportToWordButton, language);
        SendAsEmailButtonText = uiText.Get(UiTextKeys.SendAsEmailButton, language);
        CreateNewContractManufacturingButtonText = uiText.Get(UiTextKeys.CreateNewContractManufacturingButton, language);
        PdfPreviewRuntimeMissingMessage = uiText.Get(UiTextKeys.PdfPreviewRuntimeMissing, language);

        if (!_hasLoadedProposal)
        {
            ManufacturingSteps = uiText.Get(UiTextKeys.ProposalWorkspaceEmptySteps, language);
            SuggestedQuotation = uiText.Get(UiTextKeys.ProposalWorkspaceEmptyQuotation, language);
            ReferencedDocuments = uiText.Get(UiTextKeys.ProposalWorkspaceEmptyDocuments, language);
            DocumentPreviewStatus = uiText.Get(UiTextKeys.ProposalWorkspacePreviewPlaceholder, language);
            PreviewPdfPath = null;
            PreviewPlaceholderMessage = DocumentPreviewStatus;
            StatusMessage = string.Empty;
            CanOpenDrawing = false;
            CanOpenProjectFolder = false;
            CanExport = false;
            CanCreateNewContractManufacturing = false;
            Heading = uiText.Get(UiTextKeys.ProposalWorkspaceHeading, language);
            Subtitle = uiText.Get(UiTextKeys.ProposalWorkspaceSubtitle, language);
        }
        else
        {
            if (_sourceMode == ProposalSourceMode.NewContractManufacturing)
            {
                Heading = uiText.Get(UiTextKeys.ContractManufacturingWorkspaceHeading, language);
                Subtitle = string.IsNullOrWhiteSpace(_referenceProjectNumber)
                    ? uiText.Get(UiTextKeys.ContractManufacturingWorkspaceSubtitle, language)
                    : uiText.Format(
                        UiTextKeys.ContractManufacturingWorkspaceSubtitleWithReferenceFormat,
                        language,
                        _referenceProjectNumber);
            }
            else
            {
                Heading = uiText.Get(UiTextKeys.ProposalWorkspaceHeading, language);
                Subtitle = uiText.Get(UiTextKeys.ProposalWorkspaceSubtitle, language);
            }

            UpdateDrawingPreview(CanOpenDrawing);
            UpdateExportState();
        }
    }

    [RelayCommand(CanExecute = nameof(CanCreateNewContractManufacturing))]
    private void CreateNewContractManufacturing()
    {
        if (_analysisResult is null)
        {
            return;
        }

        LoadNewContractManufacturing(new NewContractManufacturingRequest
        {
            Analysis = _analysisResult,
            ReferenceProject = null,
        });
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
        var drawingPath = _sourceMode == ProposalSourceMode.NewContractManufacturing
            ? _customerDrawingPath
            : Path.Combine(_primaryProjectFolder, ProjectDocumentFileNames.Drawing);

        if (string.IsNullOrWhiteSpace(drawingPath))
        {
            return;
        }

        if (!ShellLauncher.TryOpenFile(drawingPath, out var error))
        {
            StatusMessage = error ?? uiText.Get(UiTextKeys.OpenDrawingFailed, preferences.UiLanguage);
        }
    }

    [RelayCommand(CanExecute = nameof(CanExport))]
    private void ExportToPdf()
    {
        ExportDocument(
            uiText.Get(UiTextKeys.ExportPdfDialogTitle, preferences.UiLanguage),
            uiText.Get(UiTextKeys.ExportPdfDialogFilter, preferences.UiLanguage),
            "pdf",
            pdfExporter.Export);
    }

    [RelayCommand(CanExecute = nameof(CanExport))]
    private void ExportToWord()
    {
        ExportDocument(
            uiText.Get(UiTextKeys.ExportWordDialogTitle, preferences.UiLanguage),
            uiText.Get(UiTextKeys.ExportWordDialogFilter, preferences.UiLanguage),
            "docx",
            wordExporter.Export);
    }

    [RelayCommand(CanExecute = nameof(CanSendAsEmail))]
    private async Task SendAsEmailAsync()
    {
        if (_analysisResult is null || !inboxSession.HasSourceMessage)
        {
            return;
        }

        var language = preferences.UiLanguage;
        var tempPath = Path.Combine(
            Path.GetTempPath(),
            $"contract-manufacturing-{DateTime.Now:yyyyMMdd-HHmmss}.pdf");

        var document = _sourceMode == ProposalSourceMode.NewContractManufacturing
            ? exportContentFactory.BuildNewContractManufacturing(
                _analysisResult,
                ManufacturingSteps,
                SuggestedQuotation,
                ReferencedDocuments,
                language,
                aiOptions.Value.DefaultChatModel,
                _referenceProjectNumber)
            : exportContentFactory.Build(
                _analysisResult,
                _primaryMatch!,
                ManufacturingSteps,
                SuggestedQuotation,
                ReferencedDocuments,
                language,
                aiOptions.Value.DefaultChatModel);

        var exportResult = pdfExporter.Export(document, tempPath);
        if (exportResult.IsFailure)
        {
            StatusMessage = uiText.Format(
                UiTextKeys.ExportFailedFormat,
                language,
                string.Join("; ", exportResult.Errors));
            return;
        }

        var subject = inboxSession.OriginalSubject?.StartsWith("Re:", StringComparison.OrdinalIgnoreCase) == true
            ? inboxSession.OriginalSubject
            : $"Re: {inboxSession.OriginalSubject}";

        var sendResult = await dispatcher.SendAsync(new SendProposalReplyCommand(
            inboxSession.InboxMessageId!,
            subject ?? "Re: Quotation",
            SuggestedQuotation,
            tempPath));

        StatusMessage = sendResult.IsSuccess
            ? uiText.Format(UiTextKeys.EmailSentStatusFormat, language, inboxSession.ReplyToAddress ?? string.Empty)
            : string.Join("; ", sendResult.Errors);
    }

    private void ExportDocument(
        string dialogTitle,
        string dialogFilter,
        string defaultExtension,
        Func<Export.Domain.ProposalExportDocument, string, SharedKernel.Results.Result> export)
    {
        if (_analysisResult is null)
        {
            return;
        }

        var language = preferences.UiLanguage;
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Title = dialogTitle,
            Filter = dialogFilter,
            DefaultExt = defaultExtension,
            FileName = $"contract-manufacturing-{DateTime.Now:yyyyMMdd-HHmm}.{defaultExtension}",
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        var document = _sourceMode == ProposalSourceMode.NewContractManufacturing
            ? exportContentFactory.BuildNewContractManufacturing(
                _analysisResult,
                ManufacturingSteps,
                SuggestedQuotation,
                ReferencedDocuments,
                language,
                aiOptions.Value.DefaultChatModel,
                _referenceProjectNumber)
            : exportContentFactory.Build(
                _analysisResult,
                _primaryMatch!,
                ManufacturingSteps,
                SuggestedQuotation,
                ReferencedDocuments,
                language,
                aiOptions.Value.DefaultChatModel);

        var result = export(document, dialog.FileName);
        if (!result.IsSuccess)
        {
            StatusMessage = uiText.Format(
                UiTextKeys.ExportFailedFormat,
                language,
                string.Join("; ", result.Errors));
            return;
        }

        StatusMessage = uiText.Format(UiTextKeys.ExportSucceededFormat, language, Path.GetFileName(dialog.FileName));
        if (!ShellLauncher.TryOpenFile(dialog.FileName, out var openError))
        {
            StatusMessage = openError ?? uiText.Get(UiTextKeys.ExportOpenFailed, language);
        }
    }

    private void UpdateExportState()
    {
        CanExport = _hasLoadedProposal && _analysisResult is not null;
        CanSendAsEmail = CanExport && inboxSession.HasSourceMessage;
        ExportToPdfCommand.NotifyCanExecuteChanged();
        ExportToWordCommand.NotifyCanExecuteChanged();
        SendAsEmailCommand.NotifyCanExecuteChanged();
        CreateNewContractManufacturingCommand.NotifyCanExecuteChanged();
    }

    private void UpdateCreateNewContractManufacturingState()
    {
        CanCreateNewContractManufacturing = _analysisResult is not null;
        CreateNewContractManufacturingCommand.NotifyCanExecuteChanged();
    }

    private void UpdateDrawingPreview(bool hasDrawing)
    {
        var language = preferences.UiLanguage;

        if (!hasDrawing)
        {
            PreviewPdfPath = null;
            PreviewPlaceholderMessage = uiText.Get(UiTextKeys.PdfPreviewNoDrawing, language);
            return;
        }

        if (_sourceMode == ProposalSourceMode.NewContractManufacturing)
        {
            if (string.IsNullOrWhiteSpace(_customerDrawingPath) || !File.Exists(_customerDrawingPath))
            {
                PreviewPdfPath = null;
                PreviewPlaceholderMessage = uiText.Get(UiTextKeys.PdfPreviewFileMissing, language);
                return;
            }

            PreviewPdfPath = _customerDrawingPath;
            PreviewPlaceholderMessage = string.Empty;
            return;
        }

        var drawingPath = ProjectDocumentPaths.GetDrawingPath(_primaryProjectFolder);
        if (drawingPath is null)
        {
            PreviewPdfPath = null;
            PreviewPlaceholderMessage = uiText.Get(UiTextKeys.PdfPreviewFileMissing, language);
            return;
        }

        PreviewPdfPath = drawingPath;
        PreviewPlaceholderMessage = string.Empty;
    }
}
