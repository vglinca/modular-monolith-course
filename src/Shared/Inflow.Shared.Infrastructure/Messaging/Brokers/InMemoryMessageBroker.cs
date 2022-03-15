using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using Inflow.Shared.Abstractions.Contexts;
using Inflow.Shared.Abstractions.Messaging;
using Inflow.Shared.Abstractions.Modules;
using Inflow.Shared.Infrastructure.Messaging.Contexts;
using Inflow.Shared.Infrastructure.Messaging.Dispatching;
using Inflow.Shared.Infrastructure.Messaging.Interfaces;
using Inflow.Shared.Infrastructure.Messaging.Outbox.Interfaces;
using Microsoft.Extensions.Logging;

namespace Inflow.Shared.Infrastructure.Messaging.Brokers;

internal sealed class InMemoryMessageBroker : IMessageBroker
{
    private readonly IModuleClient _moduleClient;
    private readonly MessagingOptions _messagingOptions;
    private readonly IAsyncMessageDispatcher _asyncMessageDispatcher;
    private readonly IContext _context;
    private readonly IOutboxBroker _outboxBroker;
    private readonly IMessageContextRegistry _messageContextRegistry;
    private readonly ILogger<InMemoryMessageBroker> _logger;

    public InMemoryMessageBroker(IModuleClient moduleClient, MessagingOptions messagingOptions,
        IAsyncMessageDispatcher asyncMessageDispatcher,
        ILogger<InMemoryMessageBroker> logger, IContext context, IOutboxBroker outboxBroker,
        IMessageContextRegistry messageContextRegistry)
    {
        _moduleClient = moduleClient;
        _messagingOptions = messagingOptions;
        _asyncMessageDispatcher = asyncMessageDispatcher;
        _logger = logger;
        _context = context;
        _outboxBroker = outboxBroker;
        _messageContextRegistry = messageContextRegistry;
    }

    public Task PublishAsync(IMessage message, CancellationToken cancellationToken = default)
        => PublishAsync(cancellationToken, message);

    public Task PublishAsync(IMessage[] messages, CancellationToken cancellationToken = default)
        => PublishAsync(cancellationToken, messages);

    private async Task PublishAsync(CancellationToken cancellationToken, params IMessage[] messages)
    {
        if (messages is null)
        {
            return;
        }

        messages = messages.Where(x => x is not null).ToArray();
        if (!messages.Any())
        {
            return;
        }

        foreach (var message in messages)
        {
            var messageContext = new MessageContext(Guid.NewGuid(), _context);
            _messageContextRegistry.Set(message, messageContext);

            var module = message.GetModuleName();
            var name = message.GetType().Name.Underscore();
            var requestId = _context.RequestId;
            var traceId = _context.TraceId;
            var userId = _context.IdentityContext.Id;
            var messageId = messageContext.MessageId;
            
            _logger.LogInformation("Publishing a message: {Name} ({Module}) [Request ID: {RequestId}, Message ID: {MessageId}, Trace ID: '{TraceId}', User ID: '{UserId}]...",
                name, module, requestId, messageId, traceId, userId);
        }

        if (_outboxBroker.Enabled)
        {
            await _outboxBroker.SendAsync(messages);
            return;
        }

        var tasks =
            _messagingOptions.UseASyncDispatcher
                ? messages.Select(x => _asyncMessageDispatcher.PublishAsync(x, cancellationToken))
                : messages.Select(x => _moduleClient.PublishAsync(x, cancellationToken));
        
        await Task.WhenAll(tasks);
    }
}