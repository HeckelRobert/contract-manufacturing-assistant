namespace QuotationAccelerator.Desktop.ViewModels;

using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using QuotationAccelerator.Catalog.Domain;
using QuotationAccelerator.Desktop.Resources;
using QuotationAccelerator.Desktop.Services;
using QuotationAccelerator.Matching.Application.Abstractions;
using QuotationAccelerator.Matching.Domain;
using QuotationAccelerator.SharedKernel.Configuration;
using QuotationAccelerator.SharedKernel.Enums;

public partial class ResultsViewModel(
    IMatchReasonFormatter reasonFormatter,
    IUiTextProvider uiText,
    ApplicationPreferences preferences,
    IOptions<MatchingOptions> matchingOptions) : ObservableObject
{
    public event EventHandler<ProjectMatch>? PrimaryMatchChanged;

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

    public void ApplyLocalization()
    {
        ResultsHeading = uiText.Format(
            UiTextKeys.ResultsHeadingFormat,
            preferences.UiLanguage,
            matchingOptions.Value.TopResultsCount);

        ResultsHint = uiText.Get(UiTextKeys.ResultsSelectionHint, preferences.UiLanguage);
        EmptyTitle = uiText.Get(UiTextKeys.ResultsEmptyTitle, preferences.UiLanguage);
        EmptySubtitle = uiText.Get(UiTextKeys.ResultsEmptySubtitle, preferences.UiLanguage);
        OpenFolderButtonText = uiText.Get(UiTextKeys.OpenProjectFolderButton, preferences.UiLanguage);
        OpenDrawingButtonText = uiText.Get(UiTextKeys.OpenDrawingButton, preferences.UiLanguage);
        UseForProposalButtonText = uiText.Get(UiTextKeys.UseForProposalButton, preferences.UiLanguage);

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
        Matches.Clear();
        var language = preferences.UiLanguage;
        var bestMatchBadge = uiText.Get(UiTextKeys.PrimaryMatchBadge, language);
        var selectedBadge = uiText.Get(UiTextKeys.SelectedForProposalBadge, language);

        foreach (var match in result.Matches)
        {
            var reasons = reasonFormatter.FormatAll(match.Reasons, language);
            Matches.Add(new ProjectMatchItemViewModel(
                match,
                uiText.Format(UiTextKeys.SimilarityPercentFormat, language, match.SimilarityPercent),
                string.Join(Environment.NewLine, reasons),
                bestMatchBadge,
                selectedBadge));
        }

        HasMatches = Matches.Count > 0;
        SelectedMatch = Matches.FirstOrDefault();
        StatusMessage = HasMatches
            ? string.Empty
            : uiText.Get(UiTextKeys.ResultsStatusPrompt, language);
        ApplyLocalization();
    }

    public void ClearResults()
    {
        Matches.Clear();
        SelectedMatch = null;
        HasMatches = false;
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
        }
    }
}
