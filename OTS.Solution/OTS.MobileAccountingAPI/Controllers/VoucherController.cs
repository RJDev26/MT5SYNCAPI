using Microsoft.AspNetCore.Mvc;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.DOMAIN.MobileAccoutingDTO;
using OTS.Service.Interfaces;

namespace OTS.MobileAccountingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveVoucher([FromBody] VoucherEntryRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var message = await _voucherService.SaveOrUpdateVoucherAsync(request);
                if (message.ToUpper().Contains("ERROR")) { return Ok(new { Success = false, Message = message }); }
                return Ok(new { Success = true, Message = message });
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        [HttpPost("summary")]
        public async Task<IActionResult> GetVoucherSummaries([FromBody] VoucherSummaryRequest request)
        {
            var result = await _voucherService.GetVoucherSummariesAsync(request);
            return Ok(new { success = true, data = result });
        }
        [HttpGet("getvoucher-by-id/{voucherId}")]
        public async Task<IActionResult> GetVoucherById(int voucherId)
        {
            var result = await _voucherService.GetVoucherByIdAsync(voucherId);
            if (result == null)
                return NotFound(new { Message = "Voucher not found." });

            return Ok(new { Message = "Success", Data = result });
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteVoucher([FromBody] DeleteVoucherRequest request)
        {
            var resultMessage = await _voucherService.DeleteVoucherAsync(request);

            if (resultMessage.Contains("successfully"))
                return Ok(new { success = true, message = resultMessage });

            return BadRequest(new { success = false, message = resultMessage });
        }

        [HttpPost]
        [Route("ledger")]
        public async Task<IActionResult> GetLedgerReport([FromBody] LedgerRequest request)
        {
            try
            {
                var data = await _voucherService.GetLedgerAsync(request);
                return Ok(new
                {
                    IsSuccess = true,
                    Data = data
                });
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error in GetLedgerReport");
                return StatusCode(500, new
                {
                    IsSuccess = false,
                    Message = "Internal error occurred while generating the ledger report."
                });
            }
        }
        [HttpPost]
        [Route("trial-balance")]
        public async Task<IActionResult> GetTrialBalance([FromBody] TrialBalanceRequest request)
        {
            try
            {
                var result = await _voucherService.GetTrialBalanceAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
              //  _logger.LogError(ex, "Error in GetTrialBalance");
                return StatusCode(500, new
                {
                    IsSuccess = false,
                    Message = "Internal error occurred while generating the trial balance."
                });
            }
        }


    }
}
