namespace QuotationAccelerator.Desktop.ViewModels;

using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuotationAccelerator.Catalog.Domain;
using QuotationAccelerator.Desktop.Resources;
using QuotationAccelerator.Desktop.Services;
using QuotationAccelerator.Matching.Application.Abstractions;
using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.SharedKernel.Enums;

public partial class ResultsViewModel(
    IMatchExplanationFormatter explanationFormatter,
    ProjectProfileFormatter profileFormatter,
    IUiTextProvider uiText,
    ApplicationPreferences preferences) : ObservableObject
{
    public event EventHandler<ProjectMatch>? PrimaryMatchChanged;

    public event EventHandler? UseForProposalRequested;

    private AnalyzeInquiryResult? _analysisResult;

    public ObservableCollection<ProjectMatchItemViewModel> Matches { get; } = [];

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _resultsHeading = string.Empty;

    [ObservableProperty]
    private string _resultsHint = string.Empty;

    [ObservableProperty]
    private bool _hasMatches;

    [ObservableProperty]
    private bool _hasAnalysisResult;

    [ObservableProperty]
    private string _emptyTitle = string.Empty;

    [ObservableProperty]
    private string _emptySubtitle = string.Empty;

    [ObservableProperty]
    private string _openFolderButtonText = string.Empty;

    [ObservableProperty]
    private string _openDrawingButtonText = string.Empty;

    [ObservableProperty]
    private string _useForProposalButtonText = string.Empty;

    [ObservableProperty]
    private ProjectMatchItemViewModel? _selectedMatch;

    [ObservableProperty]
    private string _drawingPreviewLabel = string.Empty;

    [ObservableProperty]
    private string? _previewPdfPath;

    [ObservableProperty]
    private string _previewPlaceholderMessage = string.Empty;

    [ObservableProperty]
    private string _pdfPreviewRuntimeMissingMessage = string.Empty;

    [ObservableProperty]
    private string _contractManufacturingHint = string.Empty;

    public void ApplyLocalization()
    {
        ResultsHeading = uiText.Get(UiTextKeys.ResultsHeadingFormat, preferences.UiLanguage);

        ResultsHint = uiText.Get(UiTextKeys.ResultsSelectionHint, preferences.UiLanguage);
        EmptyTitle = uiText.Get(UiTextKeys.ResultsEmptyTitle, preferences.UiLanguage);
        EmptySubtitle = uiText.Get(UiTextKeys.ResultsEmptySubtitle, preferences.UiLanguage);
        OpenFolderButtonText = uiText.Get(UiTextKeys.OpenProjectFolderButton, preferences.UiLanguage);
        OpenDrawingButtonText = uiText.Get(UiTextKeys.OpenDrawingButton, preferences.UiLanguage);
        UseForProposalButtonText = uiText.Get(UiTextKeys.UseForProposalButton, preferences.UiLanguage);
        ContractManufacturingHint = uiText.Get(UiTextKeys.ResultsContractManufacturingHint, preferences.UiLanguage);
        DrawingPreviewLabel = uiText.Get(UiTextKeys.ResultsDrawingPreviewLabel, preferences.UiLanguage);
        PdfPreviewRuntimeMissingMessage = uiText.Get(UiTextKeys.PdfPreviewRuntimeMissing, preferences.UiLanguage);

        foreach (var match in Matches)
        {
            match.BestMatchBadge = uiText.Get(UiTextKeys.PrimaryMatchBadge, preferences.UiLanguage);
            match.SelectedForProposalBadge = uiText.Get(UiTextKeys.SelectedForProposalBadge, preferences.UiLanguage);
        }

        if (!HasMatches)
        {
            StatusMessage = uiText.Get(UiTextKeys.ResultsStatusPrompt, preferences.UiLanguage);
        }
    }

    public void DisplayResults(AnalyzeInquiryResult result)
    {
        _analysisResult = result;
        Matches.Clear();
        var language = preferences.UiLanguage;
        var bestMatchBadge = uiText.Get(UiTextKeys.PrimaryMatchBadge, language);
        var selectedBadge = uiText.Get(UiTextKeys.SelectedForProposalBadge, language);

        foreach (var match in result.Matches)
        {
            var profileText = profileFormatter.Format(match.Project, language);
            var explanations = explanationFormatter.FormatExplanations(result.Inquiry, match, language);
            Matches.Add(new ProjectMatchItemViewModel(
                match,
                uiText.Format(UiTextKeys.SimilarityPercentFormat, language, match.SimilarityPercent),
                profileText,
                string.Join(Environment.NewLine, explanations.Select(line => $"• {line}")),
                uiText.Get(UiTextKeys.ProjectProfileSectionLabel, language),
                uiText.Get(UiTextKeys.MatchExplanationsSectionLabel, language),
                bestMatchBadge,
                selectedBadge));
        }

        HasMatches = Matches.Count > 0;
        HasAnalysisResult = true;
        SelectedMatch = Matches.FirstOrDefault();
        UpdateDrawingPreview();
        StatusMessage = HasMatches
            ? string.Empty
            : uiText.Get(UiTextKeys.ResultsStatusPrompt, language);
        ApplyLocalization();
    }

    public void ClearResults()
    {
        _analysisResult = null;
        Matches.Clear();
        SelectedMatch = null;
        HasMatches = false;
        HasAnalysisResult = false;
        PreviewPdfPath = null;
        PreviewPlaceholderMessage = string.Empty;
        ApplyLocalization();
    }

    partial void OnSelectedMatchChanged(ProjectMatchItemViewModel? value)
    {
        foreach (var match in Matches)
        {
            match.IsSelectedForProposal = ReferenceEquals(match, value);
        }

        if (value?.Match is not null)
        {
            PrimaryMatchChanged?.Invoke(this, value.Match);
        }

        UpdateDrawingPreview();
    }

    private void UpdateDrawingPreview()
    {
        var language = preferences.UiLanguage;

        if (SelectedMatch is null)
        {
            PreviewPdfPath = null;
            PreviewPlaceholderMessage = string.Empty;
            return;
        }

        if (!SelectedMatch.HasDrawing)
        {
            PreviewPdfPath = null;
            PreviewPlaceholderMessage = uiText.Get(UiTextKeys.PdfPreviewNoDrawing, language);
            return;
        }

        var drawingPath = ProjectDocumentPaths.GetDrawingPath(SelectedMatch.FolderPath);
        if (drawingPath is null)
        {
            PreviewPdfPath = null;
            PreviewPlaceholderMessage = uiText.Get(UiTextKeys.PdfPreviewFileMissing, language);
            return;
        }

        PreviewPdfPath = drawingPath;
        PreviewPlaceholderMessage = string.Empty;
    }

    [RelayCommand]
    private void OpenProjectFolder()
    {
        if (SelectedMatch is null)
        {
            return;
        }

        if (!ShellLauncher.TryOpenFolder(SelectedMatch.FolderPath, out var error))
        {
            StatusMessage = error ?? uiText.Get(UiTextKeys.OpenFolderFailed, preferences.UiLanguage);
        }
    }

    [RelayCommand]
    private void OpenDrawing()
    {
        if (SelectedMatch is null)
        {
            return;
        }

        var drawingPath = Path.Combine(SelectedMatch.FolderPath, ProjectDocumentFileNames.Drawing);
        if (!ShellLauncher.TryOpenFile(drawingPath, out var error))
        {
            StatusMessage = error ?? uiText.Get(UiTextKeys.OpenDrawingFailed, preferences.UiLanguage);
        }
    }

    [RelayCommand]
    private void UseForProposal()
    {
        if (SelectedMatch?.Match is not null)
        {
            PrimaryMatchChanged?.Invoke(this, SelectedMatch.Match);
            StatusMessage = uiText.Format(
                UiTextKeys.ProposalUpdatedFromMatchFormat,
                preferences.UiLanguage,
                SelectedMatch.ProjectNumber);
            UseForProposalRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
