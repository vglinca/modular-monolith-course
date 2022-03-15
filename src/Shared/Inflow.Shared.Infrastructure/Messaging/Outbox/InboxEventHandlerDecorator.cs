using System;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using Inflow.Shared.Abstractions.Events;
using Inflow.Shared.Abstractions.Messaging;
using Inflow.Shared.Infrastructure.Messaging.Outbox.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Inflow.Shared.Infrastructure.Messaging.Outbox;

[Decorator]
internal sealed class InboxEventHandlerDecorator<TEvent> : IEventHandler<TEvent> where TEvent : class, IEvent
{
    private readonly IEventHandler<TEvent> _handler;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageContextProvider _messageContextProvider;
    private readonly InboxTypeRegistry _inboxTypeRegistry;
    private readonly bool _enabled;

    public InboxEventHandlerDecorator(IEventHandler<TEvent> handler, IServiceProvider serviceProvider,
        IMessageContextProvider messageContextProvider, InboxTypeRegistry inboxTypeRegistry,
        OutboxOptions outboxOptions)
    {
        _handler = handler;
        _serviceProvider = serviceProvider;
        _messageContextProvider = messageContextProvider;
        _inboxTypeRegistry = inboxTypeRegistry;
        _enabled = outboxOptions.Enabled;
    }

    public async Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        if (!_enabled)
        {
            await _handler.HandleAsync(@event, cancellationToken);
            return;
        }

        var inboxType = _inboxTypeRegistry.Resolve<TEvent>();
        if (inboxType is null)
        {
            await _handler.HandleAsync(@event, cancellationToken);
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var inbox = (IInbox) scope.ServiceProvider.GetRequiredService(inboxType);
        var context = _messageContextProvider.Get(@event);
        var name = @event.GetType().Name.Underscore();
        await inbox.HandleASync(context.MessageId, name, () => _handler.HandleAsync(@event, cancellationToken));
    }
} 