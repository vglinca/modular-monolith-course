using System.Threading;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Commands;
using Inflow.Shared.Abstractions.Events;
using Inflow.Shared.Abstractions.Queries;

namespace Inflow.Shared.Abstractions.Dispatchers;

public interface IDispatcher
{
    Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;
    
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent;
    
    Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, 
        CancellationToken cancellationToken = default);
}