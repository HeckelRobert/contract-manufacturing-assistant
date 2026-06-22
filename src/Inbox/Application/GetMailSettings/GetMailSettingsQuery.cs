namespace QuotationAccelerator.Inbox.Application.GetMailSettings;

using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.SharedKernel.Abstractions;

public sealed record GetMailSettingsQuery : IQuery<MailAccountSettings>;

public sealed class GetMailSettingsHandler(IMailAccountRepository repository)
    : IQueryHandler<GetMailSettingsQuery, MailAccountSettings>
{
    public Task<MailAccountSettings> HandleAsync(GetMailSettingsQuery query, CancellationToken cancellationToken) =>
        repository.GetSettingsAsync(cancellationToken);
}
