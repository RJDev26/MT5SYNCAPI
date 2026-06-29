using Microsoft.Extensions.Options;
using OTS.WorkflowService.Options;
using OTS.WorkflowService.Services;

namespace OTS.WorkflowService
{
    /// <summary>
    /// Background worker that periodically polls MT5 for new orders and deals.
    /// Each pass syncs the window between the last high-water mark and "now".
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMt5SyncService _syncService;
        private readonly Mt5SyncOptions _options;

        public Worker(
            ILogger<Worker> logger,
            IMt5SyncService syncService,
            IOptions<Mt5SyncOptions> options)
        {
            _logger = logger;
            _syncService = syncService;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var pollInterval = TimeSpan.FromSeconds(Math.Max(1, _options.PollIntervalSeconds));

            // High-water mark: only fetch records newer than this on each pass.
            var lastSyncUtc = DateTime.UtcNow.AddMinutes(-Math.Max(0, _options.InitialLookbackMinutes));

            try
            {
                await _syncService.ConnectAsync(stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var nowUtc = DateTime.UtcNow;

                    try
                    {
                        await SyncOnceAsync(lastSyncUtc, nowUtc, stoppingToken);
                        lastSyncUtc = nowUtc;
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        // Log and keep polling; the next pass retries the same window.
                        _logger.LogError(ex, "MT5 sync pass failed; will retry next interval");
                    }

                    await Task.Delay(pollInterval, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown.
            }
            finally
            {
                await _syncService.DisconnectAsync(CancellationToken.None);
            }
        }

        private async Task SyncOnceAsync(
            DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken)
        {
            var orders = await _syncService.GetOrdersAsync(fromUtc, toUtc, cancellationToken);
            var deals = await _syncService.GetDealsAsync(fromUtc, toUtc, cancellationToken);

            _logger.LogInformation(
                "MT5 sync {FromUtc:o} -> {ToUtc:o}: {OrderCount} order(s), {DealCount} deal(s)",
                fromUtc, toUtc, orders.Count, deals.Count);

            // TODO: persist orders/deals (DB, queue, etc.) here.
        }
    }
}
