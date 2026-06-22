namespace QuotationAccelerator.Inbox.Application.DisconnectMailAccount;

using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Results;

public sealed record DisconnectMailAccountCommand : ICommand<Result>;

public sealed class DisconnectMailAccountHandler(
    IMailClient mailClient,
    IMailAccountRepository repository)
    : ICommandHandler<DisconnectMailAccountCommand, Result>
{
    public async Task<Result> HandleAsync(DisconnectMailAccountCommand command, CancellationToken cancellationToken)
    {
        await mailClient.DisconnectAsync(cancellationToken);
        await repository.SetConnectedAsync(false, cancellationToken);
        return Result.Success();
    }
}
