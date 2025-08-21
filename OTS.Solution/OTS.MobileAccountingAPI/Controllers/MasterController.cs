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

        [HttpGet("brokers")]
        public async Task<IActionResult> GetBrokers(CancellationToken ct = default)
        {
            var rows = await _masterService.GetMasterListAsync("Broker", ct);
            return Ok(rows);
        }

        [HttpGet("managers")]
        public async Task<IActionResult> GetManagers(CancellationToken ct = default)
        {
            var rows = await _masterService.GetMasterListAsync("Manager", ct);
            return Ok(rows);
        }

        [HttpGet("logins")]
        public async Task<IActionResult> GetLogins(CancellationToken ct = default)
        {
            var rows = await _masterService.GetLoginsAsync(ct);
            return Ok(rows);
        }

        [HttpGet("logins-with-client-info")]
        public async Task<IActionResult> GetLoginsWithClientInfo(
            [FromQuery] long? login,
            [FromQuery] int? managerId,
            [FromQuery] int? brokerId,
            [FromQuery] int? exId,
            [FromQuery] bool onlyWithClientRecord = false,
            CancellationToken ct = default)
        {
            var rows = await _masterService.GetMt5LoginsWithClientInfoAsync(login, managerId, brokerId, exId, onlyWithClientRecord, ct);
            return Ok(rows);
        }
    }
}
