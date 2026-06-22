namespace QuotationAccelerator.Desktop.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using QuotationAccelerator.Catalog.Application.RescanProjects;
using QuotationAccelerator.Desktop.Navigation;
using QuotationAccelerator.Desktop.Resources;
using QuotationAccelerator.Desktop.Services;
using QuotationAccelerator.Inbox.Application.ContinueContractManufacturingInquiry;
using QuotationAccelerator.Matching.Domain;

public partial class MainViewModel(
    IServiceProvider serviceProvider,
    IUiTextProvider uiText,
    ApplicationPreferences preferences,
    InboxSessionContext inboxSession) : ObservableObject
{
    [ObservableProperty]
    private int _selectedTabIndex;

    [ObservableProperty]
    private string _windowTitle = string.Empty;

    [ObservableProperty]
    private string _tabInboxHeader = string.Empty;

    [ObservableProperty]
    private string _tabInquiryHeader = string.Empty;

    [ObservableProperty]
    private string _tabResultsHeader = string.Empty;

    [ObservableProperty]
    private string _tabProposalWorkspaceHeader = string.Empty;

    [ObservableProperty]
    private string _tabSettingsHeader = string.Empty;

    public InboxViewModel Inbox { get; private set; } = null!;

    public InquiryViewModel Inquiry { get; private set; } = null!;

    public ResultsViewModel Results { get; private set; } = null!;

    public ProposalWorkspaceViewModel ProposalWorkspace { get; private set; } = null!;

    public SettingsViewModel Settings { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        Inbox = serviceProvider.GetRequiredService<InboxViewModel>();
        Inquiry = serviceProvider.GetRequiredService<InquiryViewModel>();
        Results = serviceProvider.GetRequiredService<ResultsViewModel>();
        ProposalWorkspace = serviceProvider.GetRequiredService<ProposalWorkspaceViewModel>();
        Settings = serviceProvider.GetRequiredService<SettingsViewModel>();

        Settings.Initialize();
        Inbox.Initialize();
        Inquiry.Initialize();
        Results.ApplyLocalization();
        ProposalWorkspace.Initialize();

        Settings.RescanCompleted += OnRescanCompleted;
        Settings.LocalizationChanged += OnLocalizationChanged;
        Inquiry.AnalysisCompleted += OnAnalysisCompleted;
        Results.PrimaryMatchChanged += OnPrimaryMatchChanged;
        Results.UseForProposalRequested += OnUseForProposalRequested;
        Inbox.ContinueInquiryRequested += OnContinueInquiryRequested;

        ApplyLocalization();

        await Settings.RefreshAsync();
        await Inbox.RefreshAsync();
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
        ProposalWorkspace.LoadFromAnalysis(result);
        SelectedTabIndex = (int)PrimaryTab.Results;
    }

    private void OnPrimaryMatchChanged(object? sender, ProjectMatch match)
    {
        ProposalWorkspace.LoadFromMatch(match);
    }

    private void OnUseForProposalRequested(object? sender, EventArgs e)
    {
        SelectedTabIndex = (int)PrimaryTab.ProposalWorkspace;
    }

    private void OnContinueInquiryRequested(object? sender, ContinueContractManufacturingInquiryResult result)
    {
        inboxSession.SetSourceMessage(
            result.InboxMessageId,
            result.GraphMessageId,
            result.ReplyToAddress,
            result.OriginalSubject);

        Inquiry.LoadFrom(result.Inquiry);
        SelectedTabIndex = (int)PrimaryTab.Inquiry;
    }

    private void OnLocalizationChanged(object? sender, EventArgs e)
    {
        ApplyLocalization();
        Inbox.ApplyLocalization();
        Inquiry.ApplyLocalization();
        Results.ApplyLocalization();
        ProposalWorkspace.ApplyLocalization();
        Settings.ApplyLocalization();
    }

    private void ApplyLocalization()
    {
        var language = preferences.UiLanguage;
        WindowTitle = uiText.Get(UiTextKeys.ApplicationTitle, language);
        TabInboxHeader = uiText.Get(UiTextKeys.TabInbox, language);
        TabInquiryHeader = uiText.Get(UiTextKeys.TabInquiry, language);
        TabResultsHeader = uiText.Get(UiTextKeys.TabResults, language);
        TabProposalWorkspaceHeader = uiText.Get(UiTextKeys.TabProposalWorkspace, language);
        TabSettingsHeader = uiText.Get(UiTextKeys.TabSettings, language);
    }
}
