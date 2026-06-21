namespace QuotationAccelerator.SharedKernel.Abstractions;

public interface IDispatcher
{
    Task<TResponse> SendAsync<TResponse>(
        ICommand<TResponse> command,
        CancellationToken cancellationToken = default)
        where TResponse : notnull;

    Task<TResponse> QueryAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken = default)
        where TResponse : notnull;
}
