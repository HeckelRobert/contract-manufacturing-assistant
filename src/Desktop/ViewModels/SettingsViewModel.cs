namespace QuotationAccelerator.Desktop.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using QuotationAccelerator.Catalog.Application.GetCatalogSummary;
using QuotationAccelerator.Catalog.Application.RescanProjects;
using QuotationAccelerator.Desktop.Resources;
using QuotationAccelerator.Desktop.Services;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Configuration;
using QuotationAccelerator.SharedKernel.Enums;

public partial class SettingsViewModel(
    IDispatcher dispatcher,
    IUiTextProvider uiText,
    ApplicationPreferences preferences,
    IOptions<ApplicationOptions> applicationOptions,
    IOptions<AiOptions> aiOptions) : ObservableObject
{
    public event EventHandler<RescanProjectsResult>? RescanCompleted;

    public event EventHandler? LocalizationChanged;

    public Array MatchingStrategies { get; } = Enum.GetValues<MatchingStrategy>();

    public Array UiLanguages { get; } = Enum.GetValues<UiLanguage>();

    [ObservableProperty]
    private string _projectRoot = string.Empty;

    [ObservableProperty]
    private int _indexedProjectCount;

    [ObservableProperty]
    private MatchingStrategy _matchingStrategy;

    [ObservableProperty]
    private string _chatModel = string.Empty;

    [ObservableProperty]
    private string _embeddingModel = string.Empty;

    [ObservableProperty]
    private UiLanguage _uiLanguage;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _aiProviderStatus = string.Empty;

    [ObservableProperty]
    private string _heading = string.Empty;

    [ObservableProperty]
    private string _projectCatalogGroup = string.Empty;

    [ObservableProperty]
    private string _browseButtonText = string.Empty;

    [ObservableProperty]
    private string _indexedProjectsLabel = string.Empty;

    [ObservableProperty]
    private string _rescanProjectsButtonText = string.Empty;

    [ObservableProperty]
    private string _aiProviderStatusGroup = string.Empty;

    [ObservableProperty]
    private string _matchingAndModelsGroup = string.Empty;

    [ObservableProperty]
    private string _chatModelLabel = string.Empty;

    [ObservableProperty]
    private string _embeddingModelLabel = string.Empty;

    [ObservableProperty]
    private string _matchingStrategyLabel = string.Empty;

    [ObservableProperty]
    private string _uiLanguageLabel = string.Empty;

    public void Initialize()
    {
        MatchingStrategy = applicationOptions.Value.DefaultMatchingStrategy;
        UiLanguage = applicationOptions.Value.DefaultUiLanguage;
        ChatModel = aiOptions.Value.DefaultChatModel;
        EmbeddingModel = aiOptions.Value.DefaultEmbeddingModel;
        StatusMessage = uiText.Get(UiTextKeys.SettingsStatusPrompt, UiLanguage);
        SyncPreferences();
        ApplyLocalization();
    }

    partial void OnMatchingStrategyChanged(MatchingStrategy value)
    {
        preferences.MatchingStrategy = value;
    }

    partial void OnUiLanguageChanged(UiLanguage value)
    {
        preferences.UiLanguage = value;
        ApplyLocalization();
        LocalizationChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ApplyLocalization()
    {
        var language = preferences.UiLanguage;
        Heading = uiText.Get(UiTextKeys.SettingsHeading, language);
        ProjectCatalogGroup = uiText.Get(UiTextKeys.ProjectCatalogGroup, language);
        BrowseButtonText = uiText.Get(UiTextKeys.BrowseButton, language);
        IndexedProjectsLabel = uiText.Format(UiTextKeys.IndexedProjectsFormat, language, IndexedProjectCount);
        RescanProjectsButtonText = uiText.Get(UiTextKeys.RescanProjectsButton, language);
        AiProviderStatusGroup = uiText.Get(UiTextKeys.AiProviderStatusGroup, language);
        MatchingAndModelsGroup = uiText.Get(UiTextKeys.MatchingAndModelsGroup, language);
        ChatModelLabel = uiText.Get(UiTextKeys.ChatModelLabel, language);
        EmbeddingModelLabel = uiText.Get(UiTextKeys.EmbeddingModelLabel, language);
        MatchingStrategyLabel = uiText.Get(UiTextKeys.MatchingStrategyLabel, language);
        UiLanguageLabel = uiText.Get(UiTextKeys.UiLanguageLabel, language);
        AiProviderStatus = BuildAiProviderStatus(language);
    }

    public async Task RefreshAsync()
    {
        var summary = await dispatcher.QueryAsync(new GetCatalogSummaryQuery());
        ProjectRoot = summary.ProjectRoot;
        IndexedProjectCount = summary.ProjectCount;
        IndexedProjectsLabel = uiText.Format(
            UiTextKeys.IndexedProjectsFormat,
            preferences.UiLanguage,
            IndexedProjectCount);
    }

    [RelayCommand]
    public async Task RescanAsync()
    {
        var result = await dispatcher.SendAsync(new RescanProjectsCommand(ProjectRoot));
        if (result.IsFailure)
        {
            StatusMessage = string.Join("; ", result.Errors);
            return;
        }

        IndexedProjectCount = result.Value!.ProjectCount;
        ProjectRoot = result.Value.ProjectRoot;
        StatusMessage = uiText.Format(
            UiTextKeys.RescannedProjectsFormat,
            preferences.UiLanguage,
            result.Value.ProjectCount);
        IndexedProjectsLabel = uiText.Format(
            UiTextKeys.IndexedProjectsFormat,
            preferences.UiLanguage,
            IndexedProjectCount);
        RescanCompleted?.Invoke(this, result.Value);
    }

    [RelayCommand]
    private void BrowseProjectRoot()
    {
        var dialog = new Microsoft.Win32.OpenFolderDialog
        {
            Title = uiText.Get(UiTextKeys.SelectProjectRootDialogTitle, preferences.UiLanguage),
        };

        if (dialog.ShowDialog() == true)
        {
            ProjectRoot = dialog.FolderName;
        }
    }

    private void SyncPreferences()
    {
        preferences.MatchingStrategy = MatchingStrategy;
        preferences.UiLanguage = UiLanguage;
    }

    private string BuildAiProviderStatus(UiLanguage language) =>
        string.Join(
            Environment.NewLine,
            uiText.Get(UiTextKeys.AiStatusRuleBasedAvailable, language),
            uiText.Get(UiTextKeys.AiStatusOllamaNotChecked, language),
            uiText.Get(UiTextKeys.AiStatusAzureOpenAiNotConfigured, language),
            uiText.Get(UiTextKeys.AiStatusOpenAiNotConfigured, language));
}
