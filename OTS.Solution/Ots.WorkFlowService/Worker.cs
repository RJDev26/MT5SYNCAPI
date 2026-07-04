using Microsoft.Extensions.Options;
using mtapi.mt5;

namespace Ots.WorkFlowService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly Mt5ManagerUtility _mt5ManagerUtility;
        private readonly Mt5ManagerOptions _options;

        public Worker(
            ILogger<Worker> logger,
            Mt5ManagerUtility mt5ManagerUtility,
            IOptions<Mt5ManagerOptions> options)
        {
            _logger = logger;
            _mt5ManagerUtility = mt5ManagerUtility;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var snapshot = await _mt5ManagerUtility.GetDealsAndOrdersAsync(stoppingToken);

            foreach (var order in snapshot.OpenedOrders)
            {
                LogOrder("Open order", order);
            }

            foreach (var dealOrder in snapshot.DealHistoryOrders)
            {
                LogOrder("Deal/history order", dealOrder);
            }

            _logger.LogInformation(
                "MT5 manager snapshot for {ManagerName} ({Login}) contains {OpenOrderCount} open orders, {HistoryOrderCount} deal/history orders and {InternalDealCount} internal deals.",
                _options.ManagerName,
                _options.Login,
                snapshot.OpenedOrders.Count,
                snapshot.DealHistoryOrders.Count,
                snapshot.InternalDeals.Count);
        }

        private void LogOrder(string label, Order order)
        {
            _logger.LogInformation(
                "{Label}: Ticket={Ticket}, Lots={Lots}, Symbol={Symbol}, Type={OrderType}, OpenPrice={OpenPrice}, OpenTime={OpenTime}, CloseTime={CloseTime}, Profit={Profit}.",
                label,
                order.Ticket,
                order.Lots,
                order.Symbol,
                order.OrderType,
                order.OpenPrice,
                order.OpenTime,
                order.CloseTime,
                order.Profit);
        }
    }
}
