namespace QuotationAccelerator.Desktop.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class FaqTemplateEditorViewModel : ObservableObject
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    private string _newEntryLabel = string.Empty;

    [ObservableProperty]
    private string _keywordsText = string.Empty;

    [ObservableProperty]
    private string _replyBody = string.Empty;

    public string ListDisplay => string.IsNullOrWhiteSpace(KeywordsText)
        ? _newEntryLabel
        : Truncate(KeywordsText, 80);

    public void SetNewEntryLabel(string label)
    {
        _newEntryLabel = label;
        OnPropertyChanged(nameof(ListDisplay));
    }

    partial void OnKeywordsTextChanged(string value) => OnPropertyChanged(nameof(ListDisplay));

    private static string Truncate(string text, int max) =>
        text.Length <= max ? text : string.Concat(text.AsSpan(0, max), "…");
}
