using System;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Inflow.Shared.Infrastructure.Commands;

internal sealed class InMemoryCommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    
    public InMemoryCommandDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : class, ICommand
    {
        if (command is null)
        {
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand>>();

        await handler.HandleAsync(command, cancellationToken);
    }
}