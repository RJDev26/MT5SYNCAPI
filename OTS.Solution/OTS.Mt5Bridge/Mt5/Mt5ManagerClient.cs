using System;
using System.Collections.Generic;
using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using OTS.Mt5Bridge.Contracts;

namespace OTS.Mt5Bridge.Mt5
{
    /// <summary>
    /// Thread-safe wrapper around the native MT5 Manager API. Holds a single
    /// connection for the lifetime of the bridge process and exposes order/deal
    /// queries. All native calls happen inside this net48 process.
    ///
    /// NOTE: method names (DealRequestByLogins, OrderGetByLogins, volume scaling)
    /// vary by API build. Match them to the headers in your installed package.
    /// </summary>
    public sealed class Mt5ManagerClient : IDisposable
    {
        private readonly object _gate = new object();
        private Mt5BridgeConfig _config;
        private CIMTManagerAPI? _manager;
        private bool _connected;

        public Mt5ManagerClient(Mt5BridgeConfig config) => _config = config;

        public bool IsConnected => _connected;

        public void Connect(Mt5BridgeConfig config)
        {
            lock (_gate)
            {
                DisconnectCore();
                _config = config;
                EnsureConnectedCore();
            }
        }

        public void EnsureConnected()
        {
            lock (_gate)
            {
                EnsureConnectedCore();
            }
        }

        private void EnsureConnectedCore()
        {
            if (_connected && _manager != null) return;

            var res = SMTManagerAPIFactory.Initialize(null);
            if (res != MTRetCode.MT_RET_OK)
                throw new InvalidOperationException("Factory.Initialize: " + res);

            _manager = SMTManagerAPIFactory.CreateManager(
                SMTManagerAPIFactory.ManagerAPIVersion, out res);
            if (res != MTRetCode.MT_RET_OK || _manager == null)
                throw new InvalidOperationException("CreateManager: " + res);

            var errors = new List<string>();
            foreach (var server in ServerCandidates(_config.Server))
            {
                res = _manager.Connect(
                    server,
                    _config.Login,
                    _config.Password,
                    null,
                    CIMTManagerAPI.EnPumpModes.PUMP_MODE_FULL,
                    _config.ConnectTimeoutMs);

                if (res == MTRetCode.MT_RET_OK)
                {
                    _config.Server = server;
                    _connected = true;
                    return;
                }

                errors.Add(server + " => " + res);
                _manager.Disconnect();
            }

            throw new InvalidOperationException("Connect failed for configured MT5 server(s): " + string.Join(", ", errors));
        }

        public List<OrderDto> GetOrders(IEnumerable<ulong> logins)
        {
            lock (_gate)
            {
                EnsureConnectedCore();
                var result = new List<OrderDto>();
                var orders = _manager!.OrderCreateArray();
                try
                {
                    var res = _manager.OrderGetByLogins(ToArray(logins), orders);
                    if (res != MTRetCode.MT_RET_OK)
                        throw new InvalidOperationException("OrderGetByLogins: " + res);

                    for (uint i = 0; i < orders.Total(); i++)
                    {
                        var o = orders.Next(i);
                        result.Add(new OrderDto
                        {
                            Order = o.Order(),
                            Login = o.Login(),
                            Symbol = o.Symbol(),
                            Type = (int)o.Type(),
                            VolumeInitial = o.VolumeInitial() / 10000.0,
                            VolumeCurrent = o.VolumeCurrent() / 10000.0,
                            PriceOpen = o.PriceOrder(),
                            PriceCurrent = o.PriceCurrent(),
                            TimeSetupUtc = FromUnix(o.TimeSetup()),
                            Comment = o.Comment()
                        });
                    }
                }
                finally { orders.Release(); }
                return result;
            }
        }

        public List<DealDto> GetDeals(IEnumerable<ulong> logins, DateTime fromUtc, DateTime toUtc)
        {
            lock (_gate)
            {
                EnsureConnectedCore();
                var result = new List<DealDto>();
                var deals = _manager!.DealCreateArray();
                try
                {
                    var res = _manager.DealRequestByLogins(
                        ToArray(logins), ToUnix(fromUtc), ToUnix(toUtc), deals);
                    if (res != MTRetCode.MT_RET_OK)
                        throw new InvalidOperationException("DealRequestByLogins: " + res);

                    for (uint i = 0; i < deals.Total(); i++)
                    {
                        var d = deals.Next(i);
                        result.Add(new DealDto
                        {
                            Deal = d.Deal(),
                            Order = d.Order(),
                            Login = d.Login(),
                            Symbol = d.Symbol(),
                            Entry = (int)d.Entry(),
                            Volume = d.Volume() / 10000.0,
                            Price = d.Price(),
                            Profit = d.Profit(),
                            TimeUtc = FromUnix(d.Time())
                        });
                    }
                }
                finally { deals.Release(); }
                return result;
            }
        }

        private static IEnumerable<string> ServerCandidates(string server)
        {
            if (string.IsNullOrWhiteSpace(server))
                throw new InvalidOperationException("MT5 server is required.");

            var trimmed = server.Trim();
            yield return trimmed;

            // The MT5 Manager API normally expects host:port. When users provide
            // only an IP/host, retry the standard MT5 server port as a fallback.
            if (!HasExplicitPort(trimmed))
                yield return trimmed + ":443";
        }

        private static bool HasExplicitPort(string server)
        {
            var lastColon = server.LastIndexOf(':');
            if (lastColon < 0 || lastColon == server.Length - 1) return false;

            for (var i = lastColon + 1; i < server.Length; i++)
            {
                if (!char.IsDigit(server[i])) return false;
            }

            return true;
        }

        private static ulong[] ToArray(IEnumerable<ulong> logins)
        {
            var list = new List<ulong>(logins);
            // 0 = every login visible to this manager account.
            if (list.Count == 0) list.Add(0);
            return list.ToArray();
        }

        private static long ToUnix(DateTime utc) =>
            ((DateTimeOffset)DateTime.SpecifyKind(utc, DateTimeKind.Utc)).ToUnixTimeSeconds();

        private static DateTime FromUnix(long ts) =>
            DateTimeOffset.FromUnixTimeSeconds(ts).UtcDateTime;

        public void Dispose()
        {
            lock (_gate)
            {
                DisconnectCore();
            }
        }

        private void DisconnectCore()
        {
            try
            {
                _manager?.Disconnect();
                _manager?.Release();
            }
            finally
            {
                SMTManagerAPIFactory.Shutdown();
                _connected = false;
                _manager = null;
            }
        }
    }

    public sealed class Mt5BridgeConfig
    {
        public string Server { get; set; } = string.Empty;
        public ulong Login { get; set; }
        public string Password { get; set; } = string.Empty;
        public uint ConnectTimeoutMs { get; set; } = 30000;
        public string ListenUrl { get; set; } = "http://127.0.0.1:5099";
    }
}
