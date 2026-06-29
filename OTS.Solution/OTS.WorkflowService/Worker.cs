using Microsoft.Extensions.Options;

namespace OTS.WorkflowService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly Mt5ManagerClient _mt5ManagerClient;
        private readonly Mt5ManagerOptions _options;

        public Worker(ILogger<Worker> logger, Mt5ManagerClient mt5ManagerClient, IOptions<Mt5ManagerOptions> options)
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
                    var snapshot = _mt5ManagerClient.GetTradeAndOrderSnapshot();
                    _logger.LogInformation(
                        "MT5 snapshot received from {Server}: {OrderCount} order rows and {TradeCount} trade rows.",
                        _options.Server,
                        snapshot.Orders.Count,
                        snapshot.Deals.Count);

                    foreach (var order in snapshot.Orders.Take(_options.PreviewCount > int.MaxValue ? int.MaxValue : (int)_options.PreviewCount))
                    {
                        _logger.LogInformation("MT5 order: {Order}", order);
                    }

                    foreach (var trade in snapshot.Deals.Take(_options.PreviewCount > int.MaxValue ? int.MaxValue : (int)_options.PreviewCount))
                    {
                        _logger.LogInformation("MT5 deal: {Trade}", trade);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to get MT5 trade/order snapshot from {Server}.", _options.Server);
                }

                await Task.Delay(TimeSpan.FromSeconds(Math.Max(1, _options.PollIntervalSeconds)), stoppingToken);
            }
        }
    }
}
