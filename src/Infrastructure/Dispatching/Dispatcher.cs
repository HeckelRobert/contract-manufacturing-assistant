namespace QuotationAccelerator.Infrastructure.Dispatching;

using Microsoft.Extensions.DependencyInjection;
using QuotationAccelerator.SharedKernel.Abstractions;

public sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    public Task<TResponse> SendAsync<TResponse>(
        ICommand<TResponse> command,
        CancellationToken cancellationToken = default)
        where TResponse : notnull
    {
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        dynamic handler = serviceProvider.GetRequiredService(handlerType);
        return handler.HandleAsync((dynamic)command, cancellationToken);
    }

    public Task<TResponse> QueryAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken = default)
        where TResponse : notnull
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        dynamic handler = serviceProvider.GetRequiredService(handlerType);
        return handler.HandleAsync((dynamic)query, cancellationToken);
    }
}
