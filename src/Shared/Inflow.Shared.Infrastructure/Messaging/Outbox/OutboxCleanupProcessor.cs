using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Time;
using Inflow.Shared.Infrastructure.Messaging.Outbox.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Inflow.Shared.Infrastructure.Messaging.Outbox;

internal sealed class OutboxCleanupProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IClock _clock;
    private readonly ILogger<OutboxCleanupProcessor> _logger;
    private readonly TimeSpan _interval;
    private readonly TimeSpan _startDelay;
    private readonly bool _enabled;
    private int _isProcessing;

    public OutboxCleanupProcessor(IServiceScopeFactory serviceScopeFactory, IClock clock, 
        ILogger<OutboxCleanupProcessor> logger, OutboxOptions outboxOptions)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _clock = clock;
        _logger = logger;
        _interval = outboxOptions.OutboxCleanupInterval ?? TimeSpan.FromHours(1);
        _startDelay = outboxOptions.StartDelay ?? TimeSpan.FromSeconds(10);
        _enabled = outboxOptions.Enabled;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_enabled)
        {
            return;
        }

        await Task.Delay(_startDelay, stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            if (Interlocked.Exchange(ref _isProcessing, 1) == 1)
            {
                await Task.Delay(_interval, stoppingToken);
                continue;
            }
            
            _logger.LogTrace("Started cleaning up outbox messages...");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    var outboxes = scope.ServiceProvider.GetServices<IOutbox>();
                    var tasks = outboxes.Select(o => o.CleanupAsync(_clock.CurrentDate().Subtract(_interval)));
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
                    _logger.LogTrace("Finished cleaning up outbox messages in {ElapsedMilliseconds} ms.",
                        stopwatch.ElapsedMilliseconds);
                }
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}