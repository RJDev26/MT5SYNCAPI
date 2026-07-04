using Microsoft.Extensions.Options;
using mtapi.mt5;

namespace Ots.WorkFlowService;

public sealed class Mt5ManagerUtility
{
    private readonly Mt5ManagerOptions _options;
    private readonly ILogger<Mt5ManagerUtility> _logger;

    public Mt5ManagerUtility(IOptions<Mt5ManagerOptions> options, ILogger<Mt5ManagerUtility> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task<Mt5ManagerSnapshot> GetDealsAndOrdersAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(_options.Password))
        {
            throw new InvalidOperationException("MT5 manager password is not configured. Set Mt5Manager:Password or the Mt5Manager__Password environment variable.");
        }

        var api = new MT5API(_options.Login, _options.Password, _options.Server, _options.Port);

        try
        {
            _logger.LogInformation(
                "Connecting to MT5 manager {ManagerName} ({Login}) at {Server}:{Port}.",
                _options.ManagerName,
                _options.Login,
                _options.Server,
                _options.Port);

            api.Connect();
            cancellationToken.ThrowIfCancellationRequested();

            var openedOrders = api.GetOpenedOrders() ?? Array.Empty<Order>();
            var to = DateTime.UtcNow;
            var from = to.AddDays(-Math.Max(1, _options.DealHistoryLookbackDays));
            var history = api.DownloadOrderHistory(from, to);

            var dealHistoryOrders = history?.Orders ?? Array.Empty<Order>();
            var internalDeals = ToObjectArray(history?.InternalDeals);
            var internalOrders = ToObjectArray(history?.InternalOrders);

            _logger.LogInformation(
                "MT5 sync completed. OpenOrders={OpenOrderCount}, HistoryOrders={HistoryOrderCount}, InternalDeals={InternalDealCount}, InternalOrders={InternalOrderCount}.",
                openedOrders.Length,
                dealHistoryOrders.Count,
                internalDeals.Count,
                internalOrders.Count);

            return Task.FromResult(new Mt5ManagerSnapshot(openedOrders, dealHistoryOrders, internalDeals, internalOrders));
        }
        finally
        {
            if (api.Connected)
            {
                api.Disconnect();
            }
        }
    }

    private static IReadOnlyCollection<object> ToObjectArray(System.Collections.IEnumerable? values)
    {
        if (values is null)
        {
            return Array.Empty<object>();
        }

        return values.Cast<object>().ToArray();
    }
}
