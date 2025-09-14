using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using OTS.Service.Interfaces;

namespace OTS.MobileAccountingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DealsController : ControllerBase
    {
        private readonly ILiveDealService _liveDealService;
        private readonly IOrderSnapshotService _orderSnapshotService;
        private readonly IJobbingDealService _jobbingDealService;
        private readonly IStandingService _standingService;
        private readonly ILiveSummaryService _liveSummaryService;
        private readonly IDealHistoryService _dealHistoryService;

        public DealsController(
            ILiveDealService liveDealService,
            IOrderSnapshotService orderSnapshotService,
            IJobbingDealService jobbingDealService,
            IStandingService standingService,
            ILiveSummaryService liveSummaryService,
            IDealHistoryService dealHistoryService)
        {
            _liveDealService = liveDealService;
            _orderSnapshotService = orderSnapshotService;
            _jobbingDealService = jobbingDealService;
            _standingService = standingService;
            _liveSummaryService = liveSummaryService;
            _dealHistoryService = dealHistoryService;
        }

        [HttpGet("live")]
        public async Task<IActionResult> GetLiveDeals(
            [FromQuery(Name = "date")] string date,
            [FromQuery] string? sinceTime,
            [FromQuery] string? symbol,
            [FromQuery] string? action,
            [FromQuery] int? pageSize,
            [FromQuery] bool asc = false,
            CancellationToken ct = default)
        {
            if (!DateOnly.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.None, out var onDate))
            {
                return BadRequest("Invalid date format.");
            }

            DateTime? parsedSinceTime = null;
            if (!string.IsNullOrWhiteSpace(sinceTime) && !string.Equals(sinceTime, "null", StringComparison.OrdinalIgnoreCase))
            {
                if (!DateTime.TryParse(sinceTime, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dt))
                {
                    return BadRequest("Invalid sinceTime format.");
                }

                parsedSinceTime = dt;
            }

            var result = await _liveDealService.GetLiveDealsAsync(onDate, parsedSinceTime, symbol, action, pageSize ?? 500, asc, ct);
            return Ok(new { rows = result.Rows, maxTime = result.MaxTime, rowCount = result.TotalRows });
        }

        [HttpGet("orders-snapshot")]
        public async Task<IActionResult> GetOrdersSnapshot(
            [FromQuery] string? symbol,
            [FromQuery] long? orderId,
            [FromQuery] int? top,
            CancellationToken ct = default)
        {
            var result = await _orderSnapshotService.GetOrdersSnapshotAsync(symbol, orderId, top, ct);
            return Ok(new { rows = result.Rows, maxTime = result.MaxTime, rowCount = result.TotalRows });
        }

        [HttpGet("jobbing-deals")]
        public async Task<IActionResult> GetJobbingdeals(
            [FromQuery] string? fromTime,
            [FromQuery] string? toTime,
            [FromQuery] int? intervalMinutes,
            [FromQuery] long? login,
            [FromQuery] string? symbol,
            CancellationToken ct = default)
        {
            DateTime? parsedFrom = null;
            if (!string.IsNullOrWhiteSpace(fromTime))
            {
                if (!DateTime.TryParse(fromTime, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var ft))
                {
                    return BadRequest("Invalid fromTime format.");
                }
                parsedFrom = ft;
            }

            DateTime? parsedTo = null;
            if (!string.IsNullOrWhiteSpace(toTime))
            {
                if (!DateTime.TryParse(toTime, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var tt))
                {
                    return BadRequest("Invalid toTime format.");
                }
                parsedTo = tt;
            }

            var result = await _jobbingDealService.GetJobbingDealsAsync(parsedFrom, parsedTo, intervalMinutes, login, symbol, ct);
            return Ok(new { rows = result.Rows, maxTime = result.MaxTime, rowCount = result.RowsCount });
        }

        [HttpGet("standing")]
        public async Task<IActionResult> GetStanding(
    [FromQuery(Name = "date")] string date,
    [FromQuery] long? login,
    [FromQuery] string? symbol,
    [FromQuery] string? option,
    CancellationToken ct = default)
        {
            if (!DateOnly.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.None, out var onDate))
            {
                return BadRequest("Invalid date format.");
            }

            var result = await _standingService.GetStandingAsync(onDate, login, symbol, option, ct);
            if (option == "summary")
            {
                var summary = result.Rows
        .GroupBy(s => s.Symbol)
        .Select(g => new
        {
            Symbol = g.Key,
            NetQty = g.Sum(x => x.NetQty),
            BrokerShare = g.Sum(x => x.BrokerShare),
            ManagerShare = g.Sum(x => x.ManagerShare)
        })
        .Where(x => x.NetQty != 0)
        .ToList();
                return Ok(new { rows = summary, rowCount = result.RowCount });

            }
            return Ok(new { rows = result.Rows, rowCount = result.RowCount });
        }

        [HttpGet("live-summary")]
        public async Task<IActionResult> GetLiveSummary(
            [FromQuery(Name = "from")] string from,
            [FromQuery(Name = "to")] string to,
            [FromQuery] long? managerId,
            [FromQuery] string? exchange,
            [FromQuery] string? option,
            CancellationToken ct = default)
        {
            if (!DateTime.TryParse(from, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var fromDate))
            {
                return BadRequest("Invalid from date format.");
            }

            if (!DateTime.TryParse(to, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var toDate))
            {
                return BadRequest("Invalid to date format.");
            }

            var result = await _liveSummaryService.GetLiveSummaryAsync(fromDate, toDate, managerId, exchange, option, ct);
            return Ok(new { rows = result.Rows, rowCount = result.RowCount });
        }

        [HttpGet("deal-history")]
        public async Task<IActionResult> GetDealHistory(
            [FromQuery(Name = "from")] string from,
            [FromQuery(Name = "to")] string to,
            [FromQuery] long? managerId,
            [FromQuery] long? login,
            CancellationToken ct = default)
        {
            if (!DateTime.TryParse(from, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var fromDate))
            {
                return BadRequest("Invalid from date format.");
            }

            if (!DateTime.TryParse(to, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var toDate))
            {
                return BadRequest("Invalid to date format.");
            }

            var result = await _dealHistoryService.GetDealHistoryAsync(fromDate, toDate, managerId, login, ct);
            return Ok(new { rows = result.Rows, rowCount = result.RowCount });
        }
    }
}
