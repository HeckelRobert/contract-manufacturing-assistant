namespace QuotationAccelerator.Desktop.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuotationAccelerator.Desktop.Resources;
using QuotationAccelerator.Desktop.Services;
using QuotationAccelerator.Inbox.Application.GetFaqTemplates;
using QuotationAccelerator.Inbox.Application.SaveFaqTemplates;
using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.SharedKernel.Abstractions;

public partial class MailResponsesViewModel(
    IDispatcher dispatcher,
    IUiTextProvider uiText,
    ApplicationPreferences preferences) : ObservableObject
{
    public ObservableCollection<FaqTemplateEditorViewModel> Templates { get; } = [];

    [ObservableProperty]
    private FaqTemplateEditorViewModel? _selectedTemplate;

    [ObservableProperty]
    private string _heading = string.Empty;

    [ObservableProperty]
    private string _subtitle = string.Empty;

    [ObservableProperty]
    private string _listLabel = string.Empty;

    [ObservableProperty]
    private string _selectPrompt = string.Empty;

    [ObservableProperty]
    private string _newTemplateListLabel = string.Empty;

    [ObservableProperty]
    private string _keywordsLabel = string.Empty;

    [ObservableProperty]
    private string _replyBodyLabel = string.Empty;

    [ObservableProperty]
    private string _addButtonText = string.Empty;

    [ObservableProperty]
    private string _removeButtonText = string.Empty;

    [ObservableProperty]
    private string _saveButtonText = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public bool HasSelectedTemplate => SelectedTemplate is not null;

    public void Initialize()
    {
        StatusMessage = uiText.Get(UiTextKeys.MailResponsesStatusPrompt, preferences.UiLanguage);
        ApplyLocalization();
    }

    public void ApplyLocalization()
    {
        var language = preferences.UiLanguage;
        Heading = uiText.Get(UiTextKeys.MailResponsesHeading, language);
        Subtitle = uiText.Get(UiTextKeys.MailResponsesHint, language);
        ListLabel = uiText.Get(UiTextKeys.MailResponsesListLabel, language);
        SelectPrompt = uiText.Get(UiTextKeys.MailResponsesSelectPrompt, language);
        NewTemplateListLabel = uiText.Get(UiTextKeys.MailResponseNewTemplateLabel, language);
        KeywordsLabel = uiText.Get(UiTextKeys.FaqKeywordsLabel, language);
        ReplyBodyLabel = uiText.Get(UiTextKeys.FaqReplyBodyLabel, language);
        AddButtonText = uiText.Get(UiTextKeys.AddMailResponseButton, language);
        RemoveButtonText = uiText.Get(UiTextKeys.RemoveMailResponseButton, language);
        SaveButtonText = uiText.Get(UiTextKeys.SaveFaqTemplatesButton, language);

        foreach (var template in Templates)
        {
            template.SetNewEntryLabel(NewTemplateListLabel);
        }
    }

    public async Task RefreshAsync() => await LoadTemplatesAsync();

    partial void OnSelectedTemplateChanged(FaqTemplateEditorViewModel? value) =>
        OnPropertyChanged(nameof(HasSelectedTemplate));

    [RelayCommand]
    private void AddTemplate()
    {
        var template = new FaqTemplateEditorViewModel();
        template.SetNewEntryLabel(NewTemplateListLabel);
        Templates.Add(template);
        SelectedTemplate = template;
    }

    [RelayCommand]
    private void RemoveTemplate()
    {
        if (SelectedTemplate is null)
        {
            return;
        }

        var index = Templates.IndexOf(SelectedTemplate);
        Templates.Remove(SelectedTemplate);
        SelectedTemplate = Templates.Count == 0
            ? null
            : Templates[Math.Min(index, Templates.Count - 1)];
    }

    [RelayCommand]
    private async Task SaveTemplatesAsync()
    {
        var templates = Templates
            .Where(template =>
                !string.IsNullOrWhiteSpace(template.KeywordsText) &&
                !string.IsNullOrWhiteSpace(template.ReplyBody))
            .Select(template => new FaqTemplate
            {
                Id = template.Id,
                Keywords = template.KeywordsText
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToList(),
                ReplyBody = template.ReplyBody,
            })
            .ToList();

        var result = await dispatcher.SendAsync(new SaveFaqTemplatesCommand(templates));
        StatusMessage = result.IsFailure
            ? string.Join("; ", result.Errors)
            : uiText.Get(UiTextKeys.MailResponsesSavedStatus, preferences.UiLanguage);

        if (result.IsSuccess)
        {
            await LoadTemplatesAsync();
        }
    }

    private async Task LoadTemplatesAsync()
    {
        var templates = await dispatcher.QueryAsync(new GetFaqTemplatesQuery());
        Templates.Clear();
        foreach (var template in templates)
        {
            var editor = new FaqTemplateEditorViewModel
            {
                Id = template.Id,
                KeywordsText = string.Join(", ", template.Keywords),
                ReplyBody = template.ReplyBody,
            };
            editor.SetNewEntryLabel(NewTemplateListLabel);
            Templates.Add(editor);
        }

        SelectedTemplate = Templates.FirstOrDefault();
    }
}
