using Microsoft.Extensions.Options;
using OTS.UtilityService.Options;

namespace OTS.UtilityService.Services;

public sealed class Mt5TradeSyncWorker : BackgroundService
{
    private readonly IMt5ManagerClient _managerClient;
    private readonly Mt5ManagerOptions _options;
    private readonly ILogger<Mt5TradeSyncWorker> _logger;

    public Mt5TradeSyncWorker(
        IMt5ManagerClient managerClient,
        IOptions<Mt5ManagerOptions> options,
        ILogger<Mt5TradeSyncWorker> logger)
    {
        _managerClient = managerClient;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var lastSyncTime = DateTimeOffset.UtcNow.AddMinutes(-5);
        var pollInterval = TimeSpan.FromSeconds(_options.PollIntervalSeconds);

        _logger.LogInformation(
            "MT5 trade sync worker started for {Server}:{Port} manager {ManagerName} ({Login}).",
            _options.Server,
            _options.Port,
            _options.ManagerName,
            _options.Login);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var trades = await _managerClient.GetTradesAsync(lastSyncTime, stoppingToken);
                foreach (var trade in trades.OrderBy(trade => trade.Time))
                {
                    _logger.LogInformation(
                        "MT5 trade received: DealId={DealId}, Login={Login}, Symbol={Symbol}, Volume={Volume}, Price={Price}, Side={Side}, Time={Time:o}",
                        trade.DealId,
                        trade.Login,
                        trade.Symbol,
                        trade.Volume,
                        trade.Price,
                        trade.Side,
                        trade.Time);

                    if (trade.Time > lastSyncTime)
                    {
                        lastSyncTime = trade.Time;
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MT5 trade sync failed. The worker will retry in {PollIntervalSeconds} seconds.", _options.PollIntervalSeconds);
            }

            await Task.Delay(pollInterval, stoppingToken);
        }
    }
}
