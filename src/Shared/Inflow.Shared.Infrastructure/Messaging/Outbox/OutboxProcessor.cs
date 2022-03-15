using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Shared.Infrastructure.Messaging.Outbox.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Inflow.Shared.Infrastructure.Messaging.Outbox;

internal sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly TimeSpan _interval;
    private readonly TimeSpan _startDelay;
    private readonly bool _enabled;
    private int _isProcessing;

    public OutboxProcessor(IServiceScopeFactory serviceScopeFactory, ILogger<OutboxProcessor> logger, OutboxOptions 
        outboxOptions)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _enabled = outboxOptions.Enabled;
        _interval = outboxOptions.Interval ?? TimeSpan.FromSeconds(1);
        _startDelay = outboxOptions.StartDelay ?? TimeSpan.FromSeconds(10);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_enabled)
        {
            _logger.LogWarning("Outbox is disabled");
            return;
        }

        _logger.LogInformation("Outbox is enabled, start delay: {StartDelay}, interval: {Interval}", _startDelay,
            _interval);
        await Task.Delay(_startDelay, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (Interlocked.Exchange(ref _isProcessing, 1) == 1)
            {
                await Task.Delay(_interval, stoppingToken);
                continue;
            }
            
            _logger.LogTrace("Started processing outbox messages...");
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    var outboxes = scope.ServiceProvider.GetServices<IOutbox>();
                    var tasks = outboxes.Select(o => o.PublishUnsentAsync());
                    await Task.WhenAll(tasks);
                }
                catch (Exception e)
                {
                    _logger.LogError("There was an error when processing outbox.");
                    _logger.LogError(e, e.Message);
                }
                finally
                {
                    Interlocked.Exchange(ref _isProcessing, 0);
                    stopwatch.Stop();
                    _logger.LogTrace("Finished processing outbox messages in {ElapsedMilliseconds} ms.",
                        stopwatch.ElapsedMilliseconds);
                }
            }
            
            await Task.Delay(_interval, stoppingToken);
        }
    }
}