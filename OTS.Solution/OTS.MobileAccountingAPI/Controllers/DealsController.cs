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
        public async Task<IActionResult> GetLiveDeals([FromQuery] DateOnly date, [FromQuery] DateTime? sinceTime, [FromQuery] string? symbol, [FromQuery] string? action, [FromQuery] int? pageSize, [FromQuery] bool asc = false, CancellationToken ct = default)
        {
            var result = await _service.GetLiveDealsAsync(date, sinceTime, symbol, action, pageSize ?? 500, asc, ct);
            return Ok(new { rows = result.Rows, maxTime = result.MaxTime, rowCount = result.RowCount });
        }
    }
}
