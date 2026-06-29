using OTS.WorkflowService.Models;

namespace OTS.WorkflowService.Services
{
    /// <summary>
    /// Abstraction over the MT5 data source. The concrete implementation wraps
    /// the native MT5 Manager API (via a Windows interop layer) or a terminal
    /// bridge. Keeping this as an interface lets the worker and tests stay
    /// decoupled from the native plumbing.
    /// </summary>
    public interface IMt5SyncService
    {
        /// <summary>Open a connection to the MT5 server.</summary>
        Task ConnectAsync(CancellationToken cancellationToken);

        /// <summary>Fetch orders created/updated within the given UTC window.</summary>
        Task<IReadOnlyList<Mt5Order>> GetOrdersAsync(
            DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken);

        /// <summary>Fetch deals executed within the given UTC window.</summary>
        Task<IReadOnlyList<Mt5Deal>> GetDealsAsync(
            DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken);

        /// <summary>Close the connection to the MT5 server.</summary>
        Task DisconnectAsync(CancellationToken cancellationToken);
    }
}
