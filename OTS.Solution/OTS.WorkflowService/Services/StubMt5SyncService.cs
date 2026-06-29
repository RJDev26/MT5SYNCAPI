using Microsoft.Extensions.Options;
using OTS.WorkflowService.Models;
using OTS.WorkflowService.Options;

namespace OTS.WorkflowService.Services
{
    /// <summary>
    /// Placeholder MT5 sync service. Returns no data and logs the requested
    /// windows so the worker pipeline can be exercised end-to-end before the
    /// native MT5 Manager API interop is wired in.
    ///
    /// Replace the bodies of GetOrdersAsync / GetDealsAsync with real calls
    /// into the MT5 Manager API wrapper, mapping native structs to the DTOs
    /// in OTS.WorkflowService.Models.
    /// </summary>
    public class StubMt5SyncService : IMt5SyncService
    {
        private readonly ILogger<StubMt5SyncService> _logger;
        private readonly Mt5SyncOptions _options;

        public StubMt5SyncService(
            ILogger<StubMt5SyncService> logger,
            IOptions<Mt5SyncOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public Task ConnectAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "[STUB] Connecting to MT5 server {Server} as login {Login}",
                _options.Server, _options.Login);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<Mt5Order>> GetOrdersAsync(
            DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "[STUB] GetOrders {FromUtc:o} -> {ToUtc:o}", fromUtc, toUtc);
            IReadOnlyList<Mt5Order> empty = Array.Empty<Mt5Order>();
            return Task.FromResult(empty);
        }

        public Task<IReadOnlyList<Mt5Deal>> GetDealsAsync(
            DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "[STUB] GetDeals {FromUtc:o} -> {ToUtc:o}", fromUtc, toUtc);
            IReadOnlyList<Mt5Deal> empty = Array.Empty<Mt5Deal>();
            return Task.FromResult(empty);
        }

        public Task DisconnectAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[STUB] Disconnecting from MT5 server");
            return Task.CompletedTask;
        }
    }
}
