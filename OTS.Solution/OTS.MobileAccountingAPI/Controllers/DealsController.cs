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

        public DealsController(ILiveDealService liveDealService, IOrderSnapshotService orderSnapshotService, IJobbingDealService jobbingDealService)
        {
            _liveDealService = liveDealService;
            _orderSnapshotService = orderSnapshotService;
            _jobbingDealService = jobbingDealService;
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
            var rows = await _orderSnapshotService.GetOrdersSnapshotAsync(symbol, orderId, top, ct);
            return Ok(rows);
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

            var rows = await _jobbingDealService.GetJobbingDealsAsync(parsedFrom, parsedTo, intervalMinutes, login, symbol, ct);
            return Ok(rows);
        }
    }
}
