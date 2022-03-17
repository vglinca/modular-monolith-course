using System;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using Inflow.Shared.Abstractions.Contexts;
using Inflow.Shared.Abstractions.Events;
using Inflow.Shared.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Inflow.Shared.Infrastructure.Logging.Decorators;

[Decorator]
internal sealed class LoggingEventHandlerDecorator<TEvent> : IEventHandler<TEvent> where TEvent : class, IEvent
{
    private readonly IEventHandler<TEvent> _handler;
    private readonly IMessageContextProvider _messageContextProvider;
    private readonly IContext _context;
    private readonly ILogger<LoggingEventHandlerDecorator<TEvent>> _logger;

    public LoggingEventHandlerDecorator(IEventHandler<TEvent> handler, IMessageContextProvider messageContextProvider, 
        IContext context, ILogger<LoggingEventHandlerDecorator<TEvent>> logger)
    {
        _handler = handler;
        _messageContextProvider = messageContextProvider;
        _context = context;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        var module = @event.GetModuleName();
        var name = @event.GetType().Name.Underscore();
        var messageContext = _messageContextProvider.Get(@event);
        var requestId = _context.RequestId;
        var traceId = _context.TraceId;
        var userId = _context.IdentityContext?.Id;
        var messageId = messageContext?.MessageId;
        
        _logger.LogInformation("Handling an event: {CommandName} from ({Module}) module. " +
                               "[Request ID: {RequestId} Message ID: {MessageId}, Trace ID: '{TraceId}', " +
                               "User ID: '{UserId}]...", name, module, requestId, messageId, traceId, userId);

        await _handler.HandleAsync(@event, cancellationToken);
        
        _logger.LogInformation("Handled an event: {CommandName} from ({Module}) module. " +
                               "[Request ID: {RequestId} Message ID: {MessageId}, Trace ID: '{TraceId}', " +
                               "User ID: '{UserId}]...", name, module, requestId, messageId, traceId, userId);
    }
}