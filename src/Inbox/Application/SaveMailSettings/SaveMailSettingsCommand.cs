namespace QuotationAccelerator.Inbox.Application.SaveMailSettings;

using FluentValidation;
using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Results;

public sealed record SaveMailSettingsCommand(
    string TenantId,
    string ClientId,
    string MailboxAddress,
    string FolderName) : ICommand<Result>;

public sealed class SaveMailSettingsValidator : AbstractValidator<SaveMailSettingsCommand>
{
    public SaveMailSettingsValidator()
    {
        RuleFor(command => command.TenantId).NotEmpty();
        RuleFor(command => command.ClientId).NotEmpty();
        RuleFor(command => command.MailboxAddress).NotEmpty().EmailAddress();
        RuleFor(command => command.FolderName).NotEmpty();
    }
}

public sealed class SaveMailSettingsHandler(
    IMailAccountRepository repository,
    IValidator<SaveMailSettingsCommand> validator)
    : ICommandHandler<SaveMailSettingsCommand, Result>
{
    public async Task<Result> HandleAsync(SaveMailSettingsCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure(validation.Errors.Select(error => error.ErrorMessage));
        }

        var current = await repository.GetSettingsAsync(cancellationToken);
        await repository.SaveSettingsAsync(new MailAccountSettings
        {
            TenantId = command.TenantId.Trim(),
            ClientId = command.ClientId.Trim(),
            MailboxAddress = command.MailboxAddress.Trim(),
            FolderName = command.FolderName.Trim(),
            IsConnected = current.IsConnected,
            LastFetchedAt = current.LastFetchedAt,
        }, cancellationToken);

        return Result.Success();
    }
}
