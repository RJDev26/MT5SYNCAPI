using System.Text.Json;
using Ots.WorkFlowService.MetaTrader;

namespace Ots.WorkFlowService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMetaTraderManagerClient _metaTraderManagerClient;
        private readonly MetaTraderManagerOptions _options;
        private DateTime _lastSuccessfulRunUtc;

        public Worker(
            ILogger<Worker> logger,
            IMetaTraderManagerClient metaTraderManagerClient,
            IConfiguration configuration)
        {
            _logger = logger;
            _metaTraderManagerClient = metaTraderManagerClient;
            _options = configuration.GetSection(MetaTraderManagerOptions.SectionName).Get<MetaTraderManagerOptions>()
                ?? new MetaTraderManagerOptions();
            _lastSuccessfulRunUtc = NormalizeUtc(_options.HistoryFromUtc ?? DateTime.UtcNow.AddMinutes(-5));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_options.Enabled)
            {
                _logger.LogInformation("MetaTrader manager synchronization is disabled.");
                return;
            }

            if (!await CheckManagerLoginAsync(stoppingToken))
            {
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var toUtc = DateTime.UtcNow;
                    var snapshot = await _metaTraderManagerClient.GetDealsAndOrdersAsync(
                        _lastSuccessfulRunUtc,
                        toUtc,
                        _options.TradingLogin,
                        stoppingToken);

                    LogSnapshot(snapshot);
                    _lastSuccessfulRunUtc = toUtc;
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to synchronize live MT5 deals and orders.");
                }

                await Task.Delay(_options.PollInterval, stoppingToken);
            }
        }

        private async Task<bool> CheckManagerLoginAsync(CancellationToken stoppingToken)
        {
            var isLoginSuccessful = await _metaTraderManagerClient.CheckLoginAsync(stoppingToken);
            if (!isLoginSuccessful)
            {
                _logger.LogError("MT5 synchronization stopped because manager login failed.");
            }

            return isLoginSuccessful;
        }

        private void LogSnapshot(MetaTraderSnapshot snapshot)
        {
            _logger.LogInformation(
                "MT5 synchronization completed for login {TradingLogin} from {FromUtc:o} to {ToUtc:o}.",
                snapshot.TradingLogin,
                snapshot.FromUtc,
                snapshot.ToUtc);

            _logger.LogInformation("MT5 deals: {Deals}", Serialize(snapshot.Deals));
            _logger.LogInformation("MT5 order history: {Orders}", Serialize(snapshot.Orders));
            _logger.LogInformation("MT5 pending order history: {PendingOrders}", Serialize(snapshot.PendingOrders));
        }

        private static string Serialize(object? value)
        {
            if (value is null)
            {
                return "null";
            }

            try
            {
                return JsonSerializer.Serialize(value, new JsonSerializerOptions
                {
                    WriteIndented = false
                });
            }
            catch (NotSupportedException)
            {
                return value.ToString() ?? string.Empty;
            }
        }

        private static DateTime NormalizeUtc(DateTime value)
        {
            return value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
            };
        }
    }
}
