namespace QuotationAccelerator.Desktop.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuotationAccelerator.Desktop.Resources;
using QuotationAccelerator.Desktop.Services;
using QuotationAccelerator.Inbox.Application.ConnectMailAccount;
using QuotationAccelerator.Inbox.Application.ClearInboxCache;
using QuotationAccelerator.Inbox.Application.ContinueContractManufacturingInquiry;
using QuotationAccelerator.Inbox.Application.EscalateToSupport;
using QuotationAccelerator.Inbox.Application.FetchInboxMessages;
using QuotationAccelerator.Inbox.Application.GetInboxMessages;
using QuotationAccelerator.Inbox.Application.GetSupportQueue;
using QuotationAccelerator.Inbox.Application.SendAutoReply;
using QuotationAccelerator.Inbox.Application.UpdateSupportTicket;
using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.SharedKernel.Abstractions;

public partial class InboxViewModel(
    IDispatcher dispatcher,
    IUiTextProvider uiText,
    ApplicationPreferences preferences,
    InboxSessionContext inboxSession) : ObservableObject
{
    public event EventHandler<ContinueContractManufacturingInquiryResult>? ContinueInquiryRequested;

    public ObservableCollection<InboxMessageItemViewModel> Messages { get; } = [];

    public ObservableCollection<SupportTicketItemViewModel> SupportTickets { get; } = [];

    public ObservableCollection<string> FilterOptions { get; } = [];

    [ObservableProperty]
    private string _heading = string.Empty;

    [ObservableProperty]
    private string _subtitle = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _fetchMailButtonText = string.Empty;

    [ObservableProperty]
    private string _clearInboxCacheButtonText = string.Empty;

    [ObservableProperty]
    private string _continueInquiryButtonText = string.Empty;

    [ObservableProperty]
    private string _sendAutoReplyButtonText = string.Empty;

    [ObservableProperty]
    private string _escalateToSupportButtonText = string.Empty;

    [ObservableProperty]
    private string _supportQueueGroup = string.Empty;

    [ObservableProperty]
    private string _markSupportResolvedButtonText = string.Empty;

    [ObservableProperty]
    private string _inboxReplyPreviewLabel = string.Empty;

    [ObservableProperty]
    private string _selectedFilter = string.Empty;

    [ObservableProperty]
    private InboxMessageItemViewModel? _selectedMessage;

    [ObservableProperty]
    private SupportTicketItemViewModel? _selectedSupportTicket;

    [ObservableProperty]
    private string? _replyPreviewBody;

    [ObservableProperty]
    private bool _canContinueInquiry;

    [ObservableProperty]
    private bool _canSendAutoReply;

    [ObservableProperty]
    private bool _canEscalateToSupport;

    public void Initialize()
    {
        ApplyLocalization();
    }

    public void ApplyLocalization()
    {
        var language = preferences.UiLanguage;
        Heading = uiText.Get(UiTextKeys.InboxHeading, language);
        Subtitle = uiText.Get(UiTextKeys.InboxSubtitle, language);
        FetchMailButtonText = uiText.Get(UiTextKeys.FetchMailButton, language);
        ClearInboxCacheButtonText = uiText.Get(UiTextKeys.ClearInboxCacheButton, language);
        ContinueInquiryButtonText = uiText.Get(UiTextKeys.ContinueInquiryButton, language);
        SendAutoReplyButtonText = uiText.Get(UiTextKeys.SendAutoReplyButton, language);
        EscalateToSupportButtonText = uiText.Get(UiTextKeys.EscalateToSupportButton, language);
        SupportQueueGroup = uiText.Get(UiTextKeys.SupportQueueGroup, language);
        MarkSupportResolvedButtonText = uiText.Get(UiTextKeys.MarkSupportResolvedButton, language);
        InboxReplyPreviewLabel = uiText.Get(UiTextKeys.InboxReplyPreviewLabel, language);
        StatusMessage = uiText.Get(UiTextKeys.InboxStatusPrompt, language);

        FilterOptions.Clear();
        FilterOptions.Add(uiText.Get(UiTextKeys.InboxFilterAll, language));
        FilterOptions.Add(uiText.Get(UiTextKeys.InboxFilterContractManufacturing, language));
        FilterOptions.Add(uiText.Get(UiTextKeys.InboxFilterSupport, language));
        FilterOptions.Add(uiText.Get(UiTextKeys.InboxFilterAutoAnswered, language));
        SelectedFilter = FilterOptions.First();
    }

    public async Task RefreshAsync()
    {
        await LoadMessagesAsync();
        await LoadSupportTicketsAsync();
    }

    partial void OnSelectedFilterChanged(string value) => _ = RefreshAsync();

    partial void OnSelectedMessageChanged(InboxMessageItemViewModel? value)
    {
        ReplyPreviewBody = value?.SuggestedReplyBody;
        CanContinueInquiry = value?.Category == InboxMessageCategory.ContractManufacturingInquiry;
        CanSendAutoReply = value?.Category == InboxMessageCategory.AutoAnswerable;
        CanEscalateToSupport = value is not null;
    }

    [RelayCommand]
    private async Task FetchMailAsync()
    {
        var connectResult = await dispatcher.SendAsync(new ConnectMailAccountCommand());
        if (connectResult.IsFailure)
        {
            StatusMessage = string.Join("; ", connectResult.Errors);
            return;
        }

        var fetchResult = await dispatcher.SendAsync(new FetchInboxMessagesCommand());
        if (fetchResult.IsFailure)
        {
            StatusMessage = string.Join("; ", fetchResult.Errors);
            return;
        }

        await LoadMessagesAsync();
        StatusMessage = uiText.Format(
            UiTextKeys.InboxFetchedFormat,
            preferences.UiLanguage,
            fetchResult.Value!);
    }

    [RelayCommand]
    private async Task ClearInboxCacheAsync()
    {
        var language = preferences.UiLanguage;
        var confirmation = System.Windows.MessageBox.Show(
            uiText.Get(UiTextKeys.ClearInboxCacheConfirmMessage, language),
            uiText.Get(UiTextKeys.ClearInboxCacheConfirmTitle, language),
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning);

        if (confirmation != System.Windows.MessageBoxResult.Yes)
        {
            return;
        }

        var result = await dispatcher.SendAsync(new ClearInboxCacheCommand());
        if (result.IsFailure)
        {
            StatusMessage = string.Join("; ", result.Errors);
            return;
        }

        inboxSession.Clear();
        SelectedMessage = null;
        SelectedSupportTicket = null;
        ReplyPreviewBody = null;
        await LoadMessagesAsync();
        await LoadSupportTicketsAsync();
        StatusMessage = uiText.Format(
            UiTextKeys.ClearInboxCacheSuccessFormat,
            language,
            result.Value!);
    }

    [RelayCommand]
    private async Task ContinueInquiryAsync()
    {
        if (SelectedMessage is null)
        {
            return;
        }

        var result = await dispatcher.SendAsync(
            new ContinueContractManufacturingInquiryCommand(SelectedMessage.Id));

        if (result.IsFailure)
        {
            StatusMessage = string.Join("; ", result.Errors);
            return;
        }

        StatusMessage = uiText.Get(UiTextKeys.InboxPrefilledStatus, preferences.UiLanguage);
        ContinueInquiryRequested?.Invoke(this, result.Value!);
    }

    [RelayCommand]
    private async Task SendAutoReplyAsync()
    {
        if (SelectedMessage is null || string.IsNullOrWhiteSpace(ReplyPreviewBody))
        {
            return;
        }

        var result = await dispatcher.SendAsync(
            new SendAutoReplyCommand(SelectedMessage.Id, ReplyPreviewBody));

        if (result.IsFailure)
        {
            StatusMessage = string.Join("; ", result.Errors);
            return;
        }

        StatusMessage = uiText.Get(UiTextKeys.AutoReplySentStatus, preferences.UiLanguage);
        await LoadMessagesAsync();
    }

    [RelayCommand]
    private async Task EscalateToSupportAsync()
    {
        if (SelectedMessage is null)
        {
            return;
        }

        var result = await dispatcher.SendAsync(
            new EscalateToSupportCommand(SelectedMessage.Id, Notes: null));

        if (result.IsFailure)
        {
            StatusMessage = string.Join("; ", result.Errors);
            return;
        }

        await LoadMessagesAsync();
        await LoadSupportTicketsAsync();
    }

    [RelayCommand]
    private async Task MarkSupportResolvedAsync()
    {
        if (SelectedSupportTicket is null)
        {
            return;
        }

        var result = await dispatcher.SendAsync(new UpdateSupportTicketCommand(
            SelectedSupportTicket.Id,
            SupportTicketStatus.Resolved,
            SelectedSupportTicket.Notes));

        if (result.IsFailure)
        {
            StatusMessage = string.Join("; ", result.Errors);
            return;
        }

        await LoadSupportTicketsAsync();
    }

    private async Task LoadMessagesAsync()
    {
        var messages = await dispatcher.QueryAsync(new GetInboxMessagesQuery());
        Messages.Clear();

        foreach (var message in FilterMessages(messages))
        {
            Messages.Add(MapMessage(message));
        }
    }

    private async Task LoadSupportTicketsAsync()
    {
        var tickets = await dispatcher.QueryAsync(new GetSupportQueueQuery());
        SupportTickets.Clear();

        foreach (var ticket in tickets.Where(ticket => ticket.Status != SupportTicketStatus.Resolved))
        {
            SupportTickets.Add(MapTicket(ticket));
        }
    }

    private IEnumerable<InboxMessage> FilterMessages(IReadOnlyList<InboxMessage> messages)
    {
        var language = preferences.UiLanguage;
        if (SelectedFilter == uiText.Get(UiTextKeys.InboxFilterContractManufacturing, language))
        {
            return messages.Where(message => message.Category == InboxMessageCategory.ContractManufacturingInquiry);
        }

        if (SelectedFilter == uiText.Get(UiTextKeys.InboxFilterSupport, language))
        {
            return messages.Where(message => message.Category == InboxMessageCategory.SupportRequired);
        }

        if (SelectedFilter == uiText.Get(UiTextKeys.InboxFilterAutoAnswered, language))
        {
            return messages.Where(message => message.Category == InboxMessageCategory.AutoAnswerable);
        }

        return messages;
    }

    private InboxMessageItemViewModel MapMessage(InboxMessage message)
    {
        var language = preferences.UiLanguage;
        var categoryKey = message.Category switch
        {
            InboxMessageCategory.AutoAnswerable => UiTextKeys.InboxCategoryAutoAnswerable,
            InboxMessageCategory.SupportRequired => UiTextKeys.InboxCategorySupportRequired,
            InboxMessageCategory.ContractManufacturingInquiry => UiTextKeys.InboxCategoryContractManufacturing,
            _ => UiTextKeys.InboxCategoryUncategorized,
        };

        return new InboxMessageItemViewModel
        {
            Id = message.Id,
            Subject = message.Subject,
            FromDisplay = string.IsNullOrWhiteSpace(message.FromDisplayName)
                ? message.FromAddress
                : $"{message.FromDisplayName} <{message.FromAddress}>",
            ReceivedDisplay = message.ReceivedAt.ToLocalTime().ToString("g"),
            CategoryDisplay = uiText.Get(categoryKey, language),
            Category = message.Category,
            AttachmentsDisplay = message.Attachments.Count == 0
                ? "—"
                : string.Join(", ", message.Attachments.Select(attachment => attachment.FileName)),
            BodyPreview = message.BodyPreview,
            SuggestedReplyBody = message.SuggestedReplyBody,
        };
    }

    private SupportTicketItemViewModel MapTicket(SupportTicket ticket)
    {
        var language = preferences.UiLanguage;
        var statusKey = ticket.Status switch
        {
            SupportTicketStatus.InProgress => UiTextKeys.SupportTicketStatusInProgress,
            SupportTicketStatus.Resolved => UiTextKeys.SupportTicketStatusResolved,
            _ => UiTextKeys.SupportTicketStatusOpen,
        };

        return new SupportTicketItemViewModel
        {
            Id = ticket.Id,
            Subject = ticket.Subject,
            FromAddress = ticket.FromAddress,
            Status = ticket.Status,
            StatusDisplay = uiText.Get(statusKey, language),
            Notes = ticket.Notes,
        };
    }
}
