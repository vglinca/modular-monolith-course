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

internal sealed class InboxCleanupProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IClock _clock;
    private readonly ILogger<InboxCleanupProcessor> _logger;
    private readonly TimeSpan _interval;
    private readonly TimeSpan _startDelay;
    private int _isProcessing;
    private readonly bool _enabled;

    public InboxCleanupProcessor(IServiceScopeFactory serviceScopeFactory, IClock clock, 
        ILogger<InboxCleanupProcessor> logger, OutboxOptions outboxOptions)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _clock = clock;
        _logger = logger;
        _interval = outboxOptions.InboxCleanupInterval ?? TimeSpan.FromHours(1);
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
            
            _logger.LogTrace("Started cleaning up inbox messages...");
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    var inboxes = scope.ServiceProvider.GetServices<IInbox>();
                    var tasks = inboxes.Select(i => i.CleanupAsync(_clock.CurrentDate().Subtract(_interval)));
                    await Task.WhenAll(tasks);
                }
                catch (Exception e)
                {
                    _logger.LogError("There was an error when processing inbox.");
                    _logger.LogError(e, e.Message);
                }
                finally
                {
                    Interlocked.Exchange(ref _isProcessing, 0);
                    stopWatch.Stop();
                    _logger.LogTrace("Finished cleaning up inbox messages in {ElapsedMilliseconds} ms.",
                        stopWatch.ElapsedMilliseconds);
                }
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}