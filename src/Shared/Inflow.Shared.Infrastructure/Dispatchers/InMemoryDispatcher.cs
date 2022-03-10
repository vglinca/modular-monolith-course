using System.Threading;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Commands;
using Inflow.Shared.Abstractions.Dispatchers;
using Inflow.Shared.Abstractions.Events;
using Inflow.Shared.Abstractions.Queries;

namespace Inflow.Shared.Infrastructure.Dispatchers;

public class InMemoryDispatcher : IDispatcher
{
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IEventDispatcher _eventDispatcher;

    public InMemoryDispatcher(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, 
        IEventDispatcher eventDispatcher)
    {
        _queryDispatcher = queryDispatcher;
        _commandDispatcher = commandDispatcher;
        _eventDispatcher = eventDispatcher;
    }

    public Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
        => _commandDispatcher.DispatchAsync(command, cancellationToken);

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
        => _eventDispatcher.PublishAsync(@event, cancellationToken);

    public Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        => _queryDispatcher.DispatchAsync(query, cancellationToken);
}