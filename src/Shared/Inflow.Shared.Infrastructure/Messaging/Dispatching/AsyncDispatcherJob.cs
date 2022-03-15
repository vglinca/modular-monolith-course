using System;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Modules;
using Inflow.Shared.Infrastructure.Contexts;
using Inflow.Shared.Infrastructure.Messaging.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Inflow.Shared.Infrastructure.Messaging.Dispatching;

internal sealed class AsyncDispatcherJob : BackgroundService
{
    private readonly IMessageChannel _messageChannel;
    private readonly IModuleClient _moduleClient;
    private readonly ContextAccessor _contextAccessor;
    private readonly ILogger<AsyncDispatcherJob> _logger;

    public AsyncDispatcherJob(IMessageChannel messageChannel, IModuleClient moduleClient, 
        ILogger<AsyncDispatcherJob> logger, ContextAccessor contextAccessor)
    {
        _messageChannel = messageChannel;
        _moduleClient = moduleClient;
        _logger = logger;
        _contextAccessor = contextAccessor;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var envelope in _messageChannel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation(
                    "Publishing en event using async dispatcher: {Dispatcher}", envelope.Message.GetType().Name);
                _contextAccessor.Context ??= envelope.MessageContext.Context;
                await _moduleClient.PublishAsync(envelope.Message, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }
    }
}