using System;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Modules;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Inflow.Shared.Infrastructure.Messaging;

internal sealed class AsyncDispatcherJob : BackgroundService
{
    private readonly IMessageChannel _messageChannel;
    private readonly IModuleClient _moduleClient;
    private readonly ILogger<AsyncDispatcherJob> _logger;

    public AsyncDispatcherJob(IMessageChannel messageChannel, IModuleClient moduleClient, 
        ILogger<AsyncDispatcherJob> logger)
    {
        _messageChannel = messageChannel;
        _moduleClient = moduleClient;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var envelope in _messageChannel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation(
                    $"Publishing en event using async dispatcher: {envelope.Message.GetType().Name}");
                await _moduleClient.PublishAsync(envelope.Message, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }
    }
}