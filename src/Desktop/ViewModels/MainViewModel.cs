namespace QuotationAccelerator.Desktop.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using QuotationAccelerator.Catalog.Application.RescanProjects;
using QuotationAccelerator.Desktop.Navigation;
using QuotationAccelerator.Desktop.Resources;
using QuotationAccelerator.Desktop.Services;
using QuotationAccelerator.Matching.Domain;

public partial class MainViewModel(
    IServiceProvider serviceProvider,
    IUiTextProvider uiText,
    ApplicationPreferences preferences) : ObservableObject
{
    [ObservableProperty]
    private int _selectedTabIndex;

    [ObservableProperty]
    private string _windowTitle = string.Empty;

    [ObservableProperty]
    private string _tabInquiryHeader = string.Empty;

    [ObservableProperty]
    private string _tabResultsHeader = string.Empty;

    [ObservableProperty]
    private string _tabProposalWorkspaceHeader = string.Empty;

    [ObservableProperty]
    private string _tabSettingsHeader = string.Empty;

    public InquiryViewModel Inquiry { get; private set; } = null!;

    public ResultsViewModel Results { get; private set; } = null!;

    public ProposalWorkspaceViewModel ProposalWorkspace { get; private set; } = null!;

    public SettingsViewModel Settings { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        Inquiry = serviceProvider.GetRequiredService<InquiryViewModel>();
        Results = serviceProvider.GetRequiredService<ResultsViewModel>();
        ProposalWorkspace = serviceProvider.GetRequiredService<ProposalWorkspaceViewModel>();
        Settings = serviceProvider.GetRequiredService<SettingsViewModel>();

        Settings.Initialize();
        Inquiry.Initialize();
        Results.ApplyLocalization();

        Settings.RescanCompleted += OnRescanCompleted;
        Settings.LocalizationChanged += OnLocalizationChanged;
        Inquiry.AnalysisCompleted += OnAnalysisCompleted;

        ApplyLocalization();

        await Settings.RefreshAsync();
        await Settings.RescanAsync();
    }

    private void OnRescanCompleted(object? sender, RescanProjectsResult result)
    {
        Settings.StatusMessage = uiText.Format(
            UiTextKeys.IndexedProjectsStatusFormat,
            preferences.UiLanguage,
            result.ProjectCount);
    }

    private void OnAnalysisCompleted(object? sender, AnalyzeInquiryResult result)
    {
        Results.DisplayResults(result);
        SelectedTabIndex = (int)PrimaryTab.Results;
    }

    private void OnLocalizationChanged(object? sender, EventArgs e)
    {
        ApplyLocalization();
        Inquiry.ApplyLocalization();
        Results.ApplyLocalization();
    }

    private void ApplyLocalization()
    {
        var language = preferences.UiLanguage;
        WindowTitle = uiText.Get(UiTextKeys.ApplicationTitle, language);
        TabInquiryHeader = uiText.Get(UiTextKeys.TabInquiry, language);
        TabResultsHeader = uiText.Get(UiTextKeys.TabResults, language);
        TabProposalWorkspaceHeader = uiText.Get(UiTextKeys.TabProposalWorkspace, language);
        TabSettingsHeader = uiText.Get(UiTextKeys.TabSettings, language);
    }
}
