using System.Text.Json;
using Microsoft.Extensions.Options;

namespace OTS.WorkflowService
{
    public class Worker : BackgroundService
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = true
        };

        private readonly ILogger<Worker> _logger;
        private readonly IMt5ManagerClient _mt5ManagerClient;
        private readonly Mt5ConnectionOptions _options;

        public Worker(
            ILogger<Worker> logger,
            IMt5ManagerClient mt5ManagerClient,
            IOptions<Mt5ConnectionOptions> options)
        {
            _logger = logger;
            _mt5ManagerClient = mt5ManagerClient;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var snapshot = await _mt5ManagerClient.GetOrdersAndDealsAsync(stoppingToken).ConfigureAwait(false);
                    await SaveSnapshotAsync(snapshot, stoppingToken).ConfigureAwait(false);

                    _logger.LogInformation(
                        "MT5 sync completed at {SyncedAt}. Orders: {OrderCount}; Deals: {DealCount}.",
                        snapshot.SyncedAt,
                        snapshot.Orders.Count,
                        snapshot.Deals.Count);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "MT5 sync failed for server {Server}:{Port}.", _options.Server, _options.Port);
                }

                await Task.Delay(TimeSpan.FromSeconds(Math.Max(1, _options.PollIntervalSeconds)), stoppingToken).ConfigureAwait(false);
            }
        }

        private async Task SaveSnapshotAsync(Mt5SyncSnapshot snapshot, CancellationToken cancellationToken)
        {
            Directory.CreateDirectory(_options.OutputDirectory);
            var fileName = $"mt5-orders-deals-{snapshot.SyncedAt:yyyyMMddHHmmss}.json";
            var filePath = Path.Combine(_options.OutputDirectory, fileName);
            await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(snapshot, JsonOptions), cancellationToken).ConfigureAwait(false);
        }
    }
}
