namespace QuotationAccelerator.Desktop.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using QuotationAccelerator.Desktop.Navigation;
using QuotationAccelerator.Desktop.Resources;
using QuotationAccelerator.Desktop.Services;
using QuotationAccelerator.Inbox.Application.ConnectMailAccount;
using QuotationAccelerator.Inbox.Application.DisconnectMailAccount;
using QuotationAccelerator.Inbox.Application.GetMailSettings;
using QuotationAccelerator.Inbox.Application.SaveMailSettings;
using QuotationAccelerator.SharedKernel.Abstractions;

public partial class SettingsViewModel(
    IDispatcher dispatcher,
    IUiTextProvider uiText,
    ApplicationPreferences preferences,
    MailResponsesViewModel mailResponses,
    Microsoft.Extensions.Options.IOptions<SharedKernel.Configuration.ApplicationOptions> applicationOptions,
    Microsoft.Extensions.Options.IOptions<SharedKernel.Configuration.AiOptions> aiOptions) : ObservableObject
{
    public event EventHandler<Catalog.Application.RescanProjects.RescanProjectsResult>? RescanCompleted;

    public event EventHandler? LocalizationChanged;

    public MailResponsesViewModel MailResponses { get; } = mailResponses;

    public ObservableCollection<SettingsNavItemViewModel> NavItems { get; } = [];

    public bool IsGeneralSectionSelected => SelectedNavItem?.Section == Navigation.SettingsSection.General;

    public bool IsMailResponsesSectionSelected => SelectedNavItem?.Section == Navigation.SettingsSection.MailResponses;

    public Array MatchingStrategies { get; } = Enum.GetValues<SharedKernel.Enums.MatchingStrategy>();

    public Array UiLanguages { get; } = Enum.GetValues<SharedKernel.Enums.UiLanguage>();

    [ObservableProperty]
    private string _projectRoot = string.Empty;

    [ObservableProperty]
    private int _indexedProjectCount;

    [ObservableProperty]
    private SharedKernel.Enums.MatchingStrategy _matchingStrategy;

    [ObservableProperty]
    private string _chatModel = string.Empty;

    [ObservableProperty]
    private string _embeddingModel = string.Empty;

    [ObservableProperty]
    private SharedKernel.Enums.UiLanguage _uiLanguage;

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

    [ObservableProperty]
    private string _mailSettingsGroup = string.Empty;

    [ObservableProperty]
    private string _mailTenantIdLabel = string.Empty;

    [ObservableProperty]
    private string _mailClientIdLabel = string.Empty;

    [ObservableProperty]
    private string _mailMailboxLabel = string.Empty;

    [ObservableProperty]
    private string _mailFolderLabel = string.Empty;

    [ObservableProperty]
    private string _connectMailButtonText = string.Empty;

    [ObservableProperty]
    private string _disconnectMailButtonText = string.Empty;

    [ObservableProperty]
    private string _saveMailSettingsButtonText = string.Empty;

    [ObservableProperty]
    private string _mailTenantId = string.Empty;

    [ObservableProperty]
    private string _mailClientId = string.Empty;

    [ObservableProperty]
    private string _mailMailboxAddress = string.Empty;

    [ObservableProperty]
    private string _mailFolderName = "Inbox";

    [ObservableProperty]
    private string _mailConnectionStatus = string.Empty;

    [ObservableProperty]
    private bool _isMailConnected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsGeneralSectionSelected))]
    [NotifyPropertyChangedFor(nameof(IsMailResponsesSectionSelected))]
    private SettingsNavItemViewModel? _selectedNavItem;

    public void Initialize()
    {
        MatchingStrategy = applicationOptions.Value.DefaultMatchingStrategy;
        UiLanguage = applicationOptions.Value.DefaultUiLanguage;
        ChatModel = aiOptions.Value.DefaultChatModel;
        EmbeddingModel = aiOptions.Value.DefaultEmbeddingModel;
        StatusMessage = uiText.Get(UiTextKeys.SettingsStatusPrompt, UiLanguage);
        SyncPreferences();
        MailResponses.Initialize();
        ApplyLocalization();
    }

    partial void OnMatchingStrategyChanged(SharedKernel.Enums.MatchingStrategy value)
    {
        preferences.MatchingStrategy = value;
    }

    partial void OnUiLanguageChanged(SharedKernel.Enums.UiLanguage value)
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
        MailSettingsGroup = uiText.Get(UiTextKeys.MailSettingsGroup, language);
        MailTenantIdLabel = uiText.Get(UiTextKeys.MailTenantIdLabel, language);
        MailClientIdLabel = uiText.Get(UiTextKeys.MailClientIdLabel, language);
        MailMailboxLabel = uiText.Get(UiTextKeys.MailMailboxLabel, language);
        MailFolderLabel = uiText.Get(UiTextKeys.MailFolderLabel, language);
        ConnectMailButtonText = uiText.Get(UiTextKeys.ConnectMailButton, language);
        DisconnectMailButtonText = uiText.Get(UiTextKeys.DisconnectMailButton, language);
        SaveMailSettingsButtonText = uiText.Get(UiTextKeys.SaveMailSettingsButton, language);
        AiProviderStatus = BuildAiProviderStatus(language);
        MailConnectionStatus = uiText.Get(
            IsMailConnected ? UiTextKeys.MailConnectedStatus : UiTextKeys.MailDisconnectedStatus,
            language);
        RefreshNavItems();
        MailResponses.ApplyLocalization();
    }

    private void RefreshNavItems()
    {
        var language = preferences.UiLanguage;
        var selectedSection = SelectedNavItem?.Section ?? SettingsSection.General;

        NavItems.Clear();
        NavItems.Add(new SettingsNavItemViewModel
        {
            Section = SettingsSection.General,
            Label = uiText.Get(UiTextKeys.SettingsNavGeneral, language),
        });
        NavItems.Add(new SettingsNavItemViewModel
        {
            Section = SettingsSection.MailResponses,
            Label = uiText.Get(UiTextKeys.TabMailResponses, language),
        });

        SelectedNavItem = NavItems.FirstOrDefault(item => item.Section == selectedSection)
                          ?? NavItems.FirstOrDefault();
    }

    public async Task RefreshAsync()
    {
        var summary = await dispatcher.QueryAsync(new Catalog.Application.GetCatalogSummary.GetCatalogSummaryQuery());
        ProjectRoot = summary.ProjectRoot;
        IndexedProjectCount = summary.ProjectCount;
        IndexedProjectsLabel = uiText.Format(
            UiTextKeys.IndexedProjectsFormat,
            preferences.UiLanguage,
            IndexedProjectCount);

        var mailSettings = await dispatcher.QueryAsync(new GetMailSettingsQuery());
        MailTenantId = mailSettings.TenantId ?? string.Empty;
        MailClientId = mailSettings.ClientId ?? string.Empty;
        MailMailboxAddress = mailSettings.MailboxAddress ?? string.Empty;
        MailFolderName = mailSettings.FolderName;
        IsMailConnected = mailSettings.IsConnected;
        MailConnectionStatus = uiText.Get(
            IsMailConnected ? UiTextKeys.MailConnectedStatus : UiTextKeys.MailDisconnectedStatus,
            preferences.UiLanguage);
        await MailResponses.RefreshAsync();
    }

    [RelayCommand]
    public async Task RescanAsync()
    {
        var result = await dispatcher.SendAsync(new Catalog.Application.RescanProjects.RescanProjectsCommand(ProjectRoot));
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

    [RelayCommand]
    private async Task SaveMailSettingsAsync()
    {
        var result = await dispatcher.SendAsync(new SaveMailSettingsCommand(
            MailTenantId,
            MailClientId,
            MailMailboxAddress,
            MailFolderName));

        StatusMessage = result.IsFailure
            ? string.Join("; ", result.Errors)
            : uiText.Get(UiTextKeys.SaveMailSettingsButton, preferences.UiLanguage);
    }

    [RelayCommand]
    private async Task ConnectMailAsync()
    {
        var saveResult = await dispatcher.SendAsync(new SaveMailSettingsCommand(
            MailTenantId,
            MailClientId,
            MailMailboxAddress,
            MailFolderName));

        if (saveResult.IsFailure)
        {
            StatusMessage = string.Join("; ", saveResult.Errors);
            return;
        }

        var result = await dispatcher.SendAsync(new ConnectMailAccountCommand());
        if (result.IsFailure)
        {
            StatusMessage = string.Join("; ", result.Errors);
            return;
        }

        IsMailConnected = true;
        MailConnectionStatus = uiText.Get(UiTextKeys.MailConnectedStatus, preferences.UiLanguage);
    }

    [RelayCommand]
    private async Task DisconnectMailAsync()
    {
        var result = await dispatcher.SendAsync(new DisconnectMailAccountCommand());
        if (result.IsFailure)
        {
            StatusMessage = string.Join("; ", result.Errors);
            return;
        }

        IsMailConnected = false;
        MailConnectionStatus = uiText.Get(UiTextKeys.MailDisconnectedStatus, preferences.UiLanguage);
    }

    private void SyncPreferences()
    {
        preferences.MatchingStrategy = MatchingStrategy;
        preferences.UiLanguage = UiLanguage;
    }

    private string BuildAiProviderStatus(SharedKernel.Enums.UiLanguage language) =>
        string.Join(
            Environment.NewLine,
            uiText.Get(UiTextKeys.AiStatusRuleBasedAvailable, language),
            uiText.Get(UiTextKeys.AiStatusOllamaNotChecked, language),
            uiText.Get(UiTextKeys.AiStatusAzureOpenAiNotConfigured, language),
            uiText.Get(UiTextKeys.AiStatusOpenAiNotConfigured, language));
}
