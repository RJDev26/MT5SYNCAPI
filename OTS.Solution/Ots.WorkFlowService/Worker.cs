using Microsoft.Extensions.Options;
using Ots.WorkFlowService.MetaTrader;

namespace Ots.WorkFlowService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMetaTraderManagerClient _metaTraderManagerClient;
        private readonly MetaTraderManagerOptions _options;

        public Worker(
            ILogger<Worker> logger,
            IMetaTraderManagerClient metaTraderManagerClient,
            IOptions<MetaTraderManagerOptions> options)
        {
            _logger = logger;
            _metaTraderManagerClient = metaTraderManagerClient;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var pollInterval = TimeSpan.FromSeconds(Math.Max(1, _options.PollIntervalSeconds));
            var lookback = TimeSpan.FromMinutes(Math.Max(1, _options.LookbackMinutes));
            var nextFrom = DateTime.UtcNow.Subtract(lookback);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var to = DateTime.UtcNow;
                    var snapshot = await _metaTraderManagerClient.GetLiveDealsAndOrdersAsync(nextFrom, to, stoppingToken);

                    _logger.LogInformation(
                        "MT5 live snapshot {From:o} - {To:o}: {DealCount} deals, {OrderCount} orders",
                        snapshot.From,
                        snapshot.To,
                        snapshot.Deals.Count,
                        snapshot.Orders.Count);

                    foreach (var deal in snapshot.Deals)
                    {
                        _logger.LogInformation(
                            "MT5 deal {Deal} order {Order} login {Login} {Symbol} {Action} volume {Volume} price {Price} profit {Profit} time {Time:o}",
                            deal.Deal,
                            deal.Order,
                            deal.Login,
                            deal.Symbol,
                            deal.Action,
                            deal.Volume,
                            deal.Price,
                            deal.Profit,
                            deal.Time);
                    }

                    foreach (var order in snapshot.Orders)
                    {
                        _logger.LogInformation(
                            "MT5 order {Order} login {Login} {Symbol} {Type}/{State} volume {VolumeCurrent}/{VolumeInitial} price {PriceOpen} setup {TimeSetup:o}",
                            order.Order,
                            order.Login,
                            order.Symbol,
                            order.Type,
                            order.State,
                            order.VolumeCurrent,
                            order.VolumeInitial,
                            order.PriceOpen,
                            order.TimeSetup);
                    }

                    nextFrom = to;
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load live MT5 deals and orders");
                }

                await Task.Delay(pollInterval, stoppingToken);
            }
        }
    }
}
