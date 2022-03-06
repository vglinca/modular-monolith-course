using System.Threading;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Commands;
using Inflow.Shared.Abstractions.Queries;

namespace Inflow.Shared.Abstractions.Dispatchers;

public interface IDispatcher
{
    Task DispatchCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;
    
    Task<TResult> DispatchQueryAsync<TResult>(IQuery<TResult> query, 
        CancellationToken cancellationToken = default);
}