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
        private readonly ILiveDealService _service;

        public DealsController(ILiveDealService service)
        {
            _service = service;
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

            var result = await _service.GetLiveDealsAsync(onDate, parsedSinceTime, symbol, action, pageSize ?? 500, asc, ct);
            return Ok(new { rows = result.Rows, maxTime = result.MaxTime, rowCount = result.TotalRows });
        }
    }
}
