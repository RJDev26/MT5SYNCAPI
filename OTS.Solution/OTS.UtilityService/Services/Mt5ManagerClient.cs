using System.Net.Sockets;
using Microsoft.Extensions.Options;
using OTS.UtilityService.Models;
using OTS.UtilityService.Options;

namespace OTS.UtilityService.Services;

public sealed class Mt5ManagerClient : IMt5ManagerClient
{
    private readonly Mt5ManagerOptions _options;
    private readonly ILogger<Mt5ManagerClient> _logger;

    public Mt5ManagerClient(IOptions<Mt5ManagerOptions> options, ILogger<Mt5ManagerClient> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Mt5Trade>> GetTradesAsync(DateTimeOffset from, CancellationToken cancellationToken)
    {
        await VerifyServerReachableAsync(cancellationToken);

        // TODO: Replace this connectivity check with the official MetaTrader 5 Manager API adapter.
        // The native MT5 Manager API is distributed by MetaQuotes/brokers and is not included in this repository.
        // Keep the adapter behind IMt5ManagerClient so the worker does not change when the API DLL/package is added.
        _logger.LogInformation(
            "MT5 manager endpoint {Server}:{Port} is reachable for manager {ManagerName} ({Login}); trade fetch adapter is pending.",
            _options.Server,
            _options.Port,
            _options.ManagerName,
            _options.Login);

        return Array.Empty<Mt5Trade>();
    }

    private async Task VerifyServerReachableAsync(CancellationToken cancellationToken)
    {
        using var client = new TcpClient();
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(_options.ConnectionTimeoutSeconds));
        using var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);

        await client.ConnectAsync(_options.Server, _options.Port, linkedToken.Token);
    }
}
