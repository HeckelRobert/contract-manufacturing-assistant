namespace QuotationAccelerator.Desktop.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using QuotationAccelerator.Catalog.Domain;
using QuotationAccelerator.Matching.Domain;

public partial class ProjectMatchItemViewModel : ObservableObject
{
    public ProjectMatchItemViewModel(
        ProjectMatch match,
        string similarityLabel,
        string reasonsText,
        string bestMatchBadge,
        string selectedForProposalBadge)
    {
        Match = match;
        ProjectNumber = match.Project.Metadata.ProjectNumber;
        Title = match.Project.Metadata.Title;
        FolderName = match.Project.FolderName;
        FolderPath = match.Project.FolderPath;
        SimilarityPercent = match.SimilarityPercent;
        SimilarityLabel = similarityLabel;
        ReasonsText = reasonsText;
        IsBestRankedMatch = match.IsPrimaryMatch;
        BestMatchBadge = bestMatchBadge;
        SelectedForProposalBadge = selectedForProposalBadge;
        HasDrawing = match.Project.DocumentFileNames.Any(name =>
            name.Equals(ProjectDocumentFileNames.Drawing, StringComparison.OrdinalIgnoreCase));
    }

    public ProjectMatch Match { get; }

    public string ProjectNumber { get; }

    public string Title { get; }

    public string FolderName { get; }

    public string FolderPath { get; }

    public int SimilarityPercent { get; }

    public string SimilarityLabel { get; }

    public string ReasonsText { get; }

    public bool IsBestRankedMatch { get; }

    public bool HasDrawing { get; }

    [ObservableProperty]
    private string _bestMatchBadge = string.Empty;

    [ObservableProperty]
    private string _selectedForProposalBadge = string.Empty;

    [ObservableProperty]
    private bool _isSelectedForProposal;
}
