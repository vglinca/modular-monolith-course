using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Inflow.Shared.Abstractions.Messaging;
using Inflow.Shared.Abstractions.Modules;
using Inflow.Shared.Abstractions.Time;
using Inflow.Shared.Infrastructure.Contexts;
using Inflow.Shared.Infrastructure.Messaging.Contexts;
using Inflow.Shared.Infrastructure.Messaging.Dispatching;
using Inflow.Shared.Infrastructure.Messaging.Interfaces;
using Inflow.Shared.Infrastructure.Messaging.Outbox.Interfaces;
using Inflow.Shared.Infrastructure.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Inflow.Shared.Infrastructure.Messaging.Outbox;

[SuppressMessage("Usage", "CA2254:Template should be a static expression")]
internal sealed class EfOutbox<T> : IOutbox where T : DbContext
{
    private readonly T _dbContext;
    private readonly DbSet<OutboxMessage> _set;
    private readonly IClock _clock;
    private readonly IMessageContextRegistry _messageContextRegistry;
    private readonly IMessageContextProvider _messageContextProvider;
    private readonly IModuleClient _moduleClient;
    private readonly IAsyncMessageDispatcher _asyncMessageDispatcher;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly MessagingOptions _messagingOptions;
    private readonly ILogger<EfOutbox<T>> _logger;

    public EfOutbox(T dbContext, IClock clock, IMessageContextRegistry messageContextRegistry,
        IMessageContextProvider messageContextProvider, IModuleClient moduleClient,
        IAsyncMessageDispatcher asyncMessageDispatcher, IJsonSerializer jsonSerializer,
        MessagingOptions messagingOptions, ILogger<EfOutbox<T>> logger, OutboxOptions outboxOptions)
    {
        _dbContext = dbContext;
        _set = dbContext.Set<OutboxMessage>();
        _clock = clock;
        _messageContextRegistry = messageContextRegistry;
        _messageContextProvider = messageContextProvider;
        _moduleClient = moduleClient;
        _asyncMessageDispatcher = asyncMessageDispatcher;
        _jsonSerializer = jsonSerializer;
        _messagingOptions = messagingOptions;
        _logger = logger;
        Enabled = outboxOptions.Enabled;
    }

    public bool Enabled { get; }
    
    public async Task SaveAsync(params IMessage[] messages)
    {
        var module = _dbContext.GetModuleName();
        if (!Enabled)
        {
            _logger.LogWarning("Outbox is disabled for ('{Module}') module, outgoing messages won't be saved.", module);
            return;
        }
        
        if (messages is null || !messages.Any())
        {
            _logger.LogWarning("No messages have been provided to be saved to the outbox for ('{Module}').", module);
            return;
        }

        var outboxMessages = messages.Where(x => x is not null)
            .Select(x =>
            {
                var ctx = _messageContextProvider.Get(x);
                return new OutboxMessage()
                {
                    Id = ctx.MessageId,
                    Name = x.GetType().Name.Underscore(),
                    Data = _jsonSerializer.Serialize((object) x),
                    Type = x.GetType().AssemblyQualifiedName,
                    CreatedAt = _clock.CurrentDate(),
                    TraceId = ctx.Context.TraceId,
                    UserId = ctx.Context.IdentityContext.Id
                };
            }).ToArray();
        
        if (!outboxMessages.Any())
        {
            _logger.LogWarning("No messages have been provided to be saved to the outbox for ('{Module}') module.",
                module);
            return;
        }

        await _set.AddRangeAsync(outboxMessages);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Saved {Count} messages to the outbox for ('{Module}') module.", outboxMessages.Length,
            module);

    }

    public async Task PublishUnsentAsync()
    {
        var module = _dbContext.GetModuleName();
        if (!Enabled)
        {
            _logger.LogWarning("Outbox is disabled ('{Module}'), outgoing messages won't be sent.", module);
            return;
        }
            
        var unsentMessages = await _set.Where(x => x.SentAt == null).ToListAsync();
        if (!unsentMessages.Any())
        {
            _logger.LogTrace("No unsent messages found in outbox ('{Module}').", module);
            return;
        }

        _logger.LogTrace("Found {Count} unsent messages in outbox ('{Module}'), sending...", unsentMessages.Count,
            module);

        foreach (var unsentMessage in unsentMessages)
        {
            var type = Type.GetType(unsentMessage.Type);
            var message = _jsonSerializer.Deserialize(unsentMessage.Data, type) as IMessage;
            if (message is null)
            {
                _logger.LogError(
                    "Invalid message type in outbox ('{Module}'): '{TypeName}', name: '{UnsentMessageName}', " +
                    "ID: '{UnsentMessageId}' ('{Module}').", module, type.Name, unsentMessage.Name, unsentMessage.Id,
                    module);
                continue;
            }

            var messageId = unsentMessage.Id;
            var sentAt = _clock.CurrentDate();
            var name = message.GetType().Name.Underscore();
            _messageContextRegistry.Set(message,
                new MessageContext(messageId,
                    new Context(unsentMessage.TraceId, new IdentityContext(unsentMessage.UserId))));
            
            _logger.LogInformation("Publishing a message from outbox ('{Module}'): {Name} [Message ID: {MessageId}]...",
                module, name, messageId);

            if (_messagingOptions.UseASyncDispatcher)
            {
                await _asyncMessageDispatcher.PublishAsync(message);
            }
            else
            {
                await _moduleClient.PublishAsync(message);
            }

            unsentMessage.SentAt = sentAt;
            _set.Update(unsentMessage);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task CleanupAsync(DateTime? to = null)
    {
        var module = _dbContext.GetModuleName();

        if (!Enabled)
        {
            _logger.LogWarning("Outbox is disabled for ('{Module}') module. Outgoing messages won't be cleaned up",
                module);
            return;
        }

        var toDate = to ?? _clock.CurrentDate();
        var sentMessages = await _set.Where(x => x.SentAt != null && x.CreatedAt <= toDate)
            .ToListAsync();

        if (!sentMessages.Any())
        {
            _logger.LogWarning("No sent messages were found in outbox for ('{Module}') module until {ToDate}.", module,
                toDate);
            return;
        }

        _logger.LogTrace(
            "Found {Count} sent messages in outbox ('{Module}') till: {ToDate}, cleaning up...", sentMessages.Count,
            module, toDate);
        
        _set.RemoveRange(sentMessages);
        await _dbContext.SaveChangesAsync();

        _logger.LogTrace("Removed {Count} sent messages from outbox ('{Module}') till: {ToDate}.", sentMessages.Count,
            module, toDate);
    }
}