namespace QuotationAccelerator.SharedKernel.Abstractions;

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
