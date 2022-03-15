using System;
using System.Linq;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Time;
using Inflow.Shared.Infrastructure.Messaging.Outbox.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Inflow.Shared.Infrastructure.Messaging.Outbox;

internal sealed class EfInbox<T> : IInbox where T : DbContext
{
    private readonly T _dbContext;
    private readonly DbSet<InboxMessage> _set;
    private readonly IClock _clock;
    private readonly ILogger<EfInbox<T>> _logger;

    public EfInbox(T dbContext, IClock clock, ILogger<EfInbox<T>> logger, OutboxOptions outboxOptions)
    {
        _dbContext = dbContext;
        _set = dbContext.Set<InboxMessage>();
        _clock = clock;
        _logger = logger;
        Enabled = outboxOptions.Enabled;
    }

    public bool Enabled { get; }
    
    public async Task HandleASync(Guid messageId, string name, Func<Task> handler)
    {
        var module = _dbContext.GetModuleName();
        if (!Enabled)
        {
            _logger.LogWarning("Outbox is disabled for ('{Module}') module, incoming messages won't be processed.",
                module);
            return;
        }

        _logger.LogTrace("Received a message with ID: '{MessageId}' to be processed in ('{Module}') module.", messageId,
            module);

        if (await _set.AnyAsync(x => x.Id == messageId && x.ProcessedAt != null))
        {
            _logger.LogTrace("Message with ID: '{MessageId}' has already been processed ('{Module}') module.",
                messageId, module);
            return;
        }

        _logger.LogTrace("Processing a message with ID: '{MessageId}' ('{Module}') module...", messageId, module);

        var inboxMessage = new InboxMessage()
        {
            Id = messageId,
            Name = name,
            ReceivedAt = _clock.CurrentDate()
        };

        var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            await handler();
            inboxMessage.ProcessedAt = _clock.CurrentDate();
            await _set.AddAsync(inboxMessage);
            await _dbContext.SaveChangesAsync();

            if (transaction is not null)
            {
                await transaction.CommitAsync();
            }

            _logger.LogTrace("Processed a message with ID: '{MessageId}' for ('{Module}') module.", messageId, module);
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "There was an error when processing a message with ID: '{MessageId}' ('{Module}') module.", messageId,
                module);
            if (transaction is not null)
            {
                await transaction.RollbackAsync();
            }

            throw;
        }
        finally
        {
            await transaction.DisposeAsync();
        }
    }

    public async Task CleanupAsync(DateTime? to = null)
    {
        var module = _dbContext.GetModuleName();
        if (!Enabled)
        {
            _logger.LogWarning("Outbox is disabled for ('{Module}') module. Incoming messages won't be cleaned up.",
                module);
            return;
        }

        var dateTo = to ?? _clock.CurrentDate();
        var sentMessages = await _set.Where(x => x.ReceivedAt <= dateTo).ToListAsync();
        
        if (!sentMessages.Any())
        {
            _logger.LogTrace("No received messages found in inbox for ('{Module}') module until: {DateTo}.", module,
                dateTo);
            return;
        }

        _logger.LogInformation(
            "Found {Count} received messages in inbox for ('{Module}') module until: {DateTo}, cleaning up...",
            sentMessages.Count, module, dateTo);
        
        _set.RemoveRange(sentMessages);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Removed {Count} received messages from inbox for ('{Module}') module until: {DateTo}.",
            sentMessages.Count, module, dateTo);
    }
}