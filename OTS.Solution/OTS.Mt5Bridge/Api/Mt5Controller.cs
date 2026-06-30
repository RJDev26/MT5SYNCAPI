using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;
using OTS.Mt5Bridge.Contracts;
using OTS.Mt5Bridge.Mt5;

namespace OTS.Mt5Bridge.Api
{
    /// <summary>
    /// HTTP surface of the bridge. Consumed by the .NET 8 worker over localhost.
    ///   GET /health
    ///   GET /orders?logins=0
    ///   GET /deals?fromUtc=...&toUtc=...&logins=0
    /// </summary>
    public class Mt5Controller : ApiController
    {
        // Set once at startup in Program.Main.
        public static Mt5ManagerClient Client = null!;

        [HttpGet, Route("health")]
        public IHttpActionResult Health() =>
            Ok(new { status = "ok", connected = Client.IsConnected });

        [HttpPost, Route("connect")]
        public IHttpActionResult Connect([FromBody] ConnectRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Missing MT5 manager connection details.");

                if (string.IsNullOrWhiteSpace(request.server))
                    return BadRequest("server is required.");

                if (string.IsNullOrWhiteSpace(request.password))
                    return BadRequest("password is required.");

                if (!ulong.TryParse(request.account_id, out var login))
                    return BadRequest("account_id must be a numeric MT5 manager login.");

                Client.Connect(new Mt5BridgeConfig
                {
                    Server = request.server,
                    Login = login,
                    Password = request.password
                });

                return Ok(new { connected = Client.IsConnected });
            }
            catch (Exception ex)
            {
                return Ok(new { connected = Client.IsConnected, error = ex.Message });
            }
        }

        [HttpGet, Route("orders")]
        public IHttpActionResult GetOrders(string? logins = null)
        {
            try
            {
                var items = Client.GetOrders(ParseLogins(logins));
                return Ok(new SyncResponse<OrderDto> { Connected = true, Items = items });
            }
            catch (Exception ex)
            {
                return Ok(new SyncResponse<OrderDto> { Connected = Client.IsConnected, Error = ex.Message });
            }
        }

        [HttpGet, Route("deals")]
        public IHttpActionResult GetDeals(string fromUtc, string toUtc, string? logins = null)
        {
            try
            {
                var from = DateTime.Parse(fromUtc, CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
                var to = DateTime.Parse(toUtc, CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);

                var items = Client.GetDeals(ParseLogins(logins), from, to);
                return Ok(new SyncResponse<DealDto> { Connected = true, Items = items });
            }
            catch (Exception ex)
            {
                return Ok(new SyncResponse<DealDto> { Connected = Client.IsConnected, Error = ex.Message });
            }
        }

        private static IEnumerable<ulong> ParseLogins(string? logins)
        {
            var result = new List<ulong>();
            if (string.IsNullOrWhiteSpace(logins)) return result;
            foreach (var part in logins.Split(','))
                if (ulong.TryParse(part.Trim(), out var v)) result.Add(v);
            return result;
        }

        public sealed class ConnectRequest
        {
            public string account_id { get; set; } = string.Empty;
            public string password { get; set; } = string.Empty;
            public string server { get; set; } = string.Empty;
        }
    }
}
