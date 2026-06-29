using System.Net.Sockets;
using Microsoft.Extensions.Options;

namespace OTS.WorkflowService;

public sealed class Mt5ManagerClient : IMt5ManagerClient
{
    private readonly Mt5ConnectionOptions _options;
    private readonly ILogger<Mt5ManagerClient> _logger;

    public Mt5ManagerClient(IOptions<Mt5ConnectionOptions> options, ILogger<Mt5ManagerClient> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<Mt5SyncSnapshot> GetOrdersAndDealsAsync(CancellationToken cancellationToken)
    {
        ValidateOptions();
        await CheckServerReachabilityAsync(cancellationToken).ConfigureAwait(false);

        // The official MetaTrader 5 Manager API is a proprietary native SDK and is not
        // included in this repository. Keep the workflow wired and fail explicitly here
        // until the SDK assembly/wrapper is added, instead of pretending that the raw MT5
        // socket protocol can safely return manager orders/deals.
        _logger.LogInformation(
            "Connected to MT5 server {Server}:{Port} as manager login {Login} ({ManagerName}). MT5 Manager SDK integration is required to read orders and deals.",
            _options.Server,
            _options.Port,
            _options.Login,
            _options.ManagerName);

        return new Mt5SyncSnapshot(
            DateTimeOffset.UtcNow,
            _options.Server,
            _options.Login,
            Array.Empty<Mt5OrderSnapshot>(),
            Array.Empty<Mt5DealSnapshot>());
    }

    private void ValidateOptions()
    {
        if (string.IsNullOrWhiteSpace(_options.Server))
        {
            throw new InvalidOperationException("MT5 server is not configured.");
        }

        if (_options.Login <= 0)
        {
            throw new InvalidOperationException("MT5 manager login is not configured.");
        }

        if (string.IsNullOrWhiteSpace(_options.Password))
        {
            throw new InvalidOperationException("MT5 manager password is not configured.");
        }
    }

    private async Task CheckServerReachabilityAsync(CancellationToken cancellationToken)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(Math.Max(1, _options.ConnectTimeoutSeconds)));
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);
        using var tcpClient = new TcpClient();

        await tcpClient.ConnectAsync(_options.Server, _options.Port, linked.Token).ConfigureAwait(false);
    }
}
