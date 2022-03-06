using System.Threading;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Commands;
using Inflow.Shared.Abstractions.Dispatchers;
using Inflow.Shared.Abstractions.Queries;

namespace Inflow.Shared.Infrastructure.Dispatchers;

public class InMemoryDispatcher : IDispatcher
{
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICommandDispatcher _commandDispatcher;

    public InMemoryDispatcher(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
    {
        _queryDispatcher = queryDispatcher;
        _commandDispatcher = commandDispatcher;
    }

    public Task DispatchCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
        => _commandDispatcher.DispatchAsync(command, cancellationToken);

    public Task<TResult> DispatchQueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        => _queryDispatcher.DispatchAsync(query, cancellationToken);
}