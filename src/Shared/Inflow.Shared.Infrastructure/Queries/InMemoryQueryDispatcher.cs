using System;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Queries;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable PossibleNullReferenceException

namespace Inflow.Shared.Infrastructure.Queries;

internal sealed class InMemoryQueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryQueryDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public async Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, 
        CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        var handler = scope.ServiceProvider.GetService(handlerType);
        var method = handlerType.GetMethod(nameof(IQueryHandler<IQuery<TResult>, TResult>.HandleAsync));
        if (method is null)
        {
            throw new InvalidOperationException(
                $"Could find corresponding handler for query of type: {query.GetType().Name}");
        }

        return await (Task<TResult>) method.Invoke(handler, new object[] {query, cancellationToken});
    }
}