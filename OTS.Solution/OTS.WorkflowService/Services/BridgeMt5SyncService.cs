using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using OTS.WorkflowService.Models;
using OTS.WorkflowService.Options;

namespace OTS.WorkflowService.Services
{
    /// <summary>
    /// IMt5SyncService implementation that talks to the net48 OTS.Mt5Bridge
    /// process over localhost HTTP. The native MT5 Manager API never loads in
    /// this .NET 8 process.
    /// </summary>
    public class BridgeMt5SyncService : IMt5SyncService
    {
        private readonly ILogger<BridgeMt5SyncService> _logger;
        private readonly HttpClient _http;
        private readonly Mt5SyncOptions _options;

        public BridgeMt5SyncService(
            ILogger<BridgeMt5SyncService> logger,
            HttpClient http,
            IOptions<Mt5SyncOptions> options)
        {
            _logger = logger;
            _http = http;
            _options = options.Value;
        }

        public async Task ConnectAsync(CancellationToken ct)
        {
            _logger.LogInformation(
                "Connecting MT5 manager {ManagerName} ({Login}) to {Server} through bridge {BridgeBaseUrl}",
                _options.ManagerName,
                _options.Login,
                _options.Server,
                _options.BridgeBaseUrl);

            // The bridge owns the MT5 connection; just verify it is reachable.
            var health = await _http.GetFromJsonAsync<HealthDto>("health", ct);
            if (health is null)
            {
                throw new InvalidOperationException("MT5 bridge health endpoint returned an empty response.");
            }

            _logger.LogInformation(
                "MT5 bridge health: status={Status} connected={Connected}",
                health.Status, health.Connected);
        }

        public async Task<IReadOnlyList<Mt5Order>> GetOrdersAsync(
            DateTime fromUtc, DateTime toUtc, CancellationToken ct)
        {
            var url = $"orders?logins={Uri.EscapeDataString(_options.Logins)}";
            var resp = await _http.GetFromJsonAsync<SyncResponse<OrderDto>>(url, ct);
            if (resp is null) return Array.Empty<Mt5Order>();
            if (!string.IsNullOrEmpty(resp.Error))
                _logger.LogWarning("Bridge orders error: {Error}", resp.Error);

            return resp.Items.Select(o => new Mt5Order
            {
                Order = o.Order,
                Login = o.Login,
                Symbol = o.Symbol,
                Type = o.Type,
                VolumeInitial = o.VolumeInitial,
                VolumeCurrent = o.VolumeCurrent,
                PriceOpen = o.PriceOpen,
                PriceCurrent = o.PriceCurrent,
                TimeSetup = o.TimeSetupUtc,
                Comment = o.Comment
            }).ToList();
        }

        public async Task<IReadOnlyList<Mt5Deal>> GetDealsAsync(
            DateTime fromUtc, DateTime toUtc, CancellationToken ct)
        {
            var url = $"deals?fromUtc={Uri.EscapeDataString(fromUtc.ToString("o"))}&toUtc={Uri.EscapeDataString(toUtc.ToString("o"))}&logins={Uri.EscapeDataString(_options.Logins)}";
            var resp = await _http.GetFromJsonAsync<SyncResponse<DealDto>>(url, ct);
            if (resp is null) return Array.Empty<Mt5Deal>();
            if (!string.IsNullOrEmpty(resp.Error))
                _logger.LogWarning("Bridge deals error: {Error}", resp.Error);

            return resp.Items.Select(d => new Mt5Deal
            {
                Deal = d.Deal,
                Order = d.Order,
                Login = d.Login,
                Symbol = d.Symbol,
                Entry = d.Entry,
                Volume = d.Volume,
                Price = d.Price,
                Profit = d.Profit,
                Time = d.TimeUtc
            }).ToList();
        }

        public Task DisconnectAsync(CancellationToken ct) => Task.CompletedTask;

        // Client-side mirrors of the bridge wire contracts.
        private sealed class HealthDto
        {
            public string? Status { get; set; }
            public bool Connected { get; set; }
        }

        private sealed class SyncResponse<T>
        {
            public bool Connected { get; set; }
            public string? Error { get; set; }
            public List<T> Items { get; set; } = new();
        }

        private sealed class OrderDto
        {
            public ulong Order { get; set; }
            public ulong Login { get; set; }
            public string Symbol { get; set; } = string.Empty;
            public int Type { get; set; }
            public double VolumeInitial { get; set; }
            public double VolumeCurrent { get; set; }
            public double PriceOpen { get; set; }
            public double PriceCurrent { get; set; }
            public DateTime TimeSetupUtc { get; set; }
            public string Comment { get; set; } = string.Empty;
        }

        private sealed class DealDto
        {
            public ulong Deal { get; set; }
            public ulong Order { get; set; }
            public ulong Login { get; set; }
            public string Symbol { get; set; } = string.Empty;
            public int Entry { get; set; }
            public double Volume { get; set; }
            public double Price { get; set; }
            public double Profit { get; set; }
            public DateTime TimeUtc { get; set; }
        }
    }
}
