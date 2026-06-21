namespace QuotationAccelerator.Desktop.ViewModels;

public sealed class ProjectMatchItemViewModel
{
    public ProjectMatchItemViewModel(
        string projectNumber,
        string title,
        string folderName,
        int similarityPercent,
        string similarityLabel,
        string reasonsText,
        bool isPrimaryMatch,
        string primaryMatchBadge)
    {
        ProjectNumber = projectNumber;
        Title = title;
        FolderName = folderName;
        SimilarityPercent = similarityPercent;
        SimilarityLabel = similarityLabel;
        ReasonsText = reasonsText;
        IsPrimaryMatch = isPrimaryMatch;
        PrimaryMatchBadge = primaryMatchBadge;
    }

    public string ProjectNumber { get; }

    public string Title { get; }

    public string FolderName { get; }

    public int SimilarityPercent { get; }

    public string SimilarityLabel { get; }

    public string ReasonsText { get; }

    public bool IsPrimaryMatch { get; }

    public string PrimaryMatchBadge { get; }
}
