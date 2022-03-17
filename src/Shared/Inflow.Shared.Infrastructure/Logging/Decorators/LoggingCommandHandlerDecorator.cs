using System;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using Inflow.Shared.Abstractions.Commands;
using Inflow.Shared.Abstractions.Contexts;
using Inflow.Shared.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Inflow.Shared.Infrastructure.Logging.Decorators;

[Decorator]
internal sealed class LoggingCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand : class, ICommand
{
    private readonly ICommandHandler<TCommand> _handler;
    private readonly IMessageContextProvider _messageContextProvider;
    private readonly IContext _context;
    private readonly ILogger<LoggingCommandHandlerDecorator<TCommand>> _logger;

    public LoggingCommandHandlerDecorator(ICommandHandler<TCommand> handler, 
        IMessageContextProvider messageContextProvider, IContext context, 
        ILogger<LoggingCommandHandlerDecorator<TCommand>> logger)
    {
        _handler = handler;
        _messageContextProvider = messageContextProvider;
        _context = context;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var module = command.GetModuleName();
        var name = command.GetType().Name.Underscore();
        var messageContext = _messageContextProvider.Get(command);
        var requestId = _context.RequestId;
        var traceId = _context.TraceId;
        var userId = _context.IdentityContext?.Id;
        var messageId = messageContext?.MessageId;

        _logger.LogInformation("Handling a command: {CommandName} from ({Module}) module. " +
                               "[Request ID: {RequestId} Message ID: {MessageId}, Trace ID: '{TraceId}', " +
                               "User ID: '{UserId}]...", name, module, requestId, messageId, traceId, userId);

        await _handler.HandleAsync(command, cancellationToken);
        
        _logger.LogInformation("Handled a command: {CommandName} from ({Module}) module. " +
                               "[Request ID: {RequestId} Message ID: {MessageId}, Trace ID: '{TraceId}', " +
                               "User ID: '{UserId}]...", name, module, requestId, messageId, traceId, userId);
    }
}