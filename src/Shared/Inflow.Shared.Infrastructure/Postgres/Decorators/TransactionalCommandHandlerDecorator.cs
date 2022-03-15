using System;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Inflow.Shared.Infrastructure.Postgres.Decorators;

[Decorator]
internal class TransactionalCommandHandlerDecorator<T> : ICommandHandler<T> where T : class, ICommand
{
    private readonly ICommandHandler<T> _handler;
    private readonly UnitOfWorkRegistry _registry;
    private readonly IServiceProvider _serviceProvider;

    public TransactionalCommandHandlerDecorator(ICommandHandler<T> handler, UnitOfWorkRegistry registry,
        IServiceProvider serviceProvider)
    {
        _handler = handler;
        _registry = registry;
        _serviceProvider = serviceProvider;
    }
    
    public async Task HandleAsync(T command, CancellationToken cancellationToken = default)
    {
        var unitOfWorkType = _registry.Resolve<T>();
        if (unitOfWorkType is null)
        {
            await _handler.HandleAsync(command, cancellationToken);
            return;
        }

        var unitOfWork = (IUnitOfWork) _serviceProvider.GetRequiredService(unitOfWorkType);
        await unitOfWork.ExecuteAsync(() => _handler.HandleAsync(command, cancellationToken));
    }
}