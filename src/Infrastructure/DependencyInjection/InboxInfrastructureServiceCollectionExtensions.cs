namespace QuotationAccelerator.Infrastructure.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.Inbox.Application.DependencyInjection;
using QuotationAccelerator.Infrastructure.Mail;

public static class InboxInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInboxInfrastructure(this IServiceCollection services)
    {
        services.AddInboxModule();
        services.AddScoped<IMailAccountRepository, MailAccountRepository>();
        services.AddScoped<IInboxMessageRepository, InboxMessageRepository>();
        services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();
        services.AddScoped<IFaqTemplateRepository, FaqTemplateRepository>();
        services.AddSingleton<IMailAttachmentStore, MailAttachmentStore>();
        services.AddSingleton<IMailClient, GraphMailClient>();
        return services;
    }
}
