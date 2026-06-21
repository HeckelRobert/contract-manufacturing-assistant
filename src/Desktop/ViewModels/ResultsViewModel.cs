namespace QuotationAccelerator.Desktop.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Options;
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
    public ObservableCollection<ProjectMatchItemViewModel> Matches { get; } = [];

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _resultsHeading = string.Empty;

    [ObservableProperty]
    private bool _hasMatches;

    public void ApplyLocalization()
    {
        ResultsHeading = uiText.Format(
            UiTextKeys.ResultsHeadingFormat,
            preferences.UiLanguage,
            matchingOptions.Value.TopResultsCount);

        if (!HasMatches)
        {
            StatusMessage = uiText.Get(UiTextKeys.ResultsStatusPrompt, preferences.UiLanguage);
        }
    }

    public void DisplayResults(AnalyzeInquiryResult result)
    {
        Matches.Clear();

        foreach (var match in result.Matches)
        {
            var reasons = reasonFormatter.FormatAll(match.Reasons, preferences.UiLanguage);
            Matches.Add(new ProjectMatchItemViewModel(
                match.Project.Metadata.ProjectNumber,
                match.Project.Metadata.Title,
                match.Project.FolderName,
                match.SimilarityPercent,
                uiText.Format(
                    UiTextKeys.SimilarityPercentFormat,
                    preferences.UiLanguage,
                    match.SimilarityPercent),
                string.Join(Environment.NewLine, reasons),
                match.IsPrimaryMatch,
                uiText.Get(UiTextKeys.PrimaryMatchBadge, preferences.UiLanguage)));
        }

        HasMatches = Matches.Count > 0;
        StatusMessage = HasMatches
            ? string.Empty
            : uiText.Get(UiTextKeys.ResultsStatusPrompt, preferences.UiLanguage);
        ApplyLocalization();
    }

    public void ClearResults()
    {
        Matches.Clear();
        HasMatches = false;
        ApplyLocalization();
    }
}
