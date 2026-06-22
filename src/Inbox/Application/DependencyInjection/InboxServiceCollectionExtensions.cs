namespace QuotationAccelerator.Inbox.Application.DependencyInjection;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.Inbox.Application.Categorization;
using QuotationAccelerator.Inbox.Application.ClearInboxCache;
using QuotationAccelerator.Inbox.Application.ConnectMailAccount;
using QuotationAccelerator.Inbox.Application.ContinueContractManufacturingInquiry;
using QuotationAccelerator.Inbox.Application.DisconnectMailAccount;
using QuotationAccelerator.Inbox.Application.EscalateToSupport;
using QuotationAccelerator.Inbox.Application.Extraction;
using QuotationAccelerator.Inbox.Application.FetchInboxMessages;
using QuotationAccelerator.Inbox.Application.GetFaqTemplates;
using QuotationAccelerator.Inbox.Application.GetInboxMessages;
using QuotationAccelerator.Inbox.Application.GetMailSettings;
using QuotationAccelerator.Inbox.Application.GetSupportQueue;
using QuotationAccelerator.Inbox.Application.SaveFaqTemplates;
using QuotationAccelerator.Inbox.Application.SaveMailSettings;
using QuotationAccelerator.Inbox.Application.SendAutoReply;
using QuotationAccelerator.Inbox.Application.SendProposalReply;
using QuotationAccelerator.Inbox.Application.UpdateSupportTicket;
using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Results;

public static class InboxServiceCollectionExtensions
{
    public static IServiceCollection AddInboxModule(this IServiceCollection services)
    {
        services.AddScoped<IValidator<SaveMailSettingsCommand>, SaveMailSettingsValidator>();
        services.AddScoped<ICommandHandler<SaveMailSettingsCommand, Result>, SaveMailSettingsHandler>();
        services.AddScoped<IQueryHandler<GetMailSettingsQuery, MailAccountSettings>, GetMailSettingsHandler>();

        services.AddScoped<ICommandHandler<ConnectMailAccountCommand, Result>, ConnectMailAccountHandler>();
        services.AddScoped<ICommandHandler<DisconnectMailAccountCommand, Result>, DisconnectMailAccountHandler>();

        services.AddScoped<IQueryHandler<GetInboxMessagesQuery, IReadOnlyList<InboxMessage>>, GetInboxMessagesHandler>();
        services.AddScoped<ICommandHandler<FetchInboxMessagesCommand, Result<int>>, FetchInboxMessagesHandler>();
        services.AddScoped<ICommandHandler<ClearInboxCacheCommand, Result<int>>, ClearInboxCacheHandler>();

        services.AddScoped<IValidator<SendAutoReplyCommand>, SendAutoReplyValidator>();
        services.AddScoped<ICommandHandler<SendAutoReplyCommand, Result>, SendAutoReplyHandler>();

        services.AddScoped<IValidator<EscalateToSupportCommand>, EscalateToSupportValidator>();
        services.AddScoped<ICommandHandler<EscalateToSupportCommand, Result>, EscalateToSupportHandler>();

        services.AddScoped<IValidator<UpdateSupportTicketCommand>, UpdateSupportTicketValidator>();
        services.AddScoped<ICommandHandler<UpdateSupportTicketCommand, Result>, UpdateSupportTicketHandler>();

        services.AddScoped<IQueryHandler<GetSupportQueueQuery, IReadOnlyList<SupportTicket>>, GetSupportQueueHandler>();

        services.AddScoped<IValidator<ContinueContractManufacturingInquiryCommand>, ContinueContractManufacturingInquiryValidator>();
        services.AddScoped<ICommandHandler<ContinueContractManufacturingInquiryCommand, Result<ContinueContractManufacturingInquiryResult>>, ContinueContractManufacturingInquiryHandler>();

        services.AddScoped<IValidator<SendProposalReplyCommand>, SendProposalReplyValidator>();
        services.AddScoped<ICommandHandler<SendProposalReplyCommand, Result>, SendProposalReplyHandler>();

        services.AddScoped<IQueryHandler<GetFaqTemplatesQuery, IReadOnlyList<FaqTemplate>>, GetFaqTemplatesHandler>();
        services.AddScoped<IValidator<SaveFaqTemplatesCommand>, SaveFaqTemplatesValidator>();
        services.AddScoped<ICommandHandler<SaveFaqTemplatesCommand, Result>, SaveFaqTemplatesHandler>();

        services.AddSingleton<IEmailCategorizationService, RuleBasedEmailCategorizationService>();
        services.AddSingleton<IInquiryExtractionService, HeuristicInquiryExtractionService>();

        return services;
    }
}
