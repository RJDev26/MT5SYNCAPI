using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.Service.Interfaces;

namespace OTS.MobileAccountingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MasterController : ControllerBase
    {
        private readonly IMasterService _masterService;

        public MasterController(IMasterService masterService)
        {
            _masterService = masterService;
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrUpdate([FromBody] MasterRequestVM request, CancellationToken ct = default)
        {
            var response = await _masterService.SaveOrUpdateAsync(request, ct);
            if (response.Success == 1)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string tableName, [FromQuery] int id, CancellationToken ct = default)
        {
            var response = await _masterService.DeleteAsync(tableName, id, ct);
            if (response.Success == 1)
                return Ok(response);

            return BadRequest(response);
        }
    }
}
