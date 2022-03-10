using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Messaging;
using Inflow.Shared.Abstractions.Modules;
using Microsoft.Extensions.Logging;

namespace Inflow.Shared.Infrastructure.Messaging;

internal sealed class InMemoryMessageBroker : IMessageBroker
{
    private readonly IModuleClient _moduleClient;
    private readonly MessagingOptions _messagingOptions;
    private readonly IAsyncMessageDispatcher _asyncMessageDispatcher;
    private readonly ILogger<InMemoryMessageBroker> _logger;

    public InMemoryMessageBroker(IModuleClient moduleClient, MessagingOptions messagingOptions, 
        IAsyncMessageDispatcher asyncMessageDispatcher, 
        ILogger<InMemoryMessageBroker> logger)
    {
        _moduleClient = moduleClient;
        _messagingOptions = messagingOptions;
        _asyncMessageDispatcher = asyncMessageDispatcher;
        _logger = logger;
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

        var tasks =
            _messagingOptions.UseASyncDispatcher
                ? messages.Select(x => _asyncMessageDispatcher.PublishAsync(x, cancellationToken))
                : messages.Select(x => _moduleClient.PublishAsync(x, cancellationToken));
        
        await Task.WhenAll(tasks);
    }
}