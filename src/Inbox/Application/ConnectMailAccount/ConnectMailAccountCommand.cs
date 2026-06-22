namespace QuotationAccelerator.Inbox.Application.ConnectMailAccount;

using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Results;

public sealed record ConnectMailAccountCommand : ICommand<Result>;

public sealed class ConnectMailAccountHandler(
    IMailClient mailClient,
    IMailAccountRepository repository)
    : ICommandHandler<ConnectMailAccountCommand, Result>
{
    public async Task<Result> HandleAsync(ConnectMailAccountCommand command, CancellationToken cancellationToken)
    {
        var settings = await repository.GetSettingsAsync(cancellationToken);
        if (!settings.IsConfigured)
        {
            return Result.Failure("Mail account is not configured.");
        }

        var connectResult = await mailClient.ConnectInteractiveAsync(cancellationToken);
        if (connectResult.IsFailure)
        {
            return connectResult;
        }

        await repository.SetConnectedAsync(true, cancellationToken);
        return Result.Success();
    }
}
