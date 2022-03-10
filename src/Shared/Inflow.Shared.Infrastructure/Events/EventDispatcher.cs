using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Inflow.Shared.Infrastructure.Events;

internal sealed class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public EventDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class, IEvent
    {
        using var scope = _serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>();
        var tasks = handlers.Select(h => h.HandleAsync(@event, cancellationToken));
        await Task.WhenAll(tasks);
    }
}