using Microsoft.AspNetCore.Mvc;
using MobileAccounting.Entities;
using MobileAccounting.Services.Interfaces;
using OTS.DOMAIN.MobileAccoutingDTO;

namespace OTS.MobileAccountingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountMasterController : ControllerBase
    {
        private readonly IAccountMasterService _accountService;

        public AccountMasterController(IAccountMasterService accountService)
        {
            _accountService = accountService;
        }

        
        [HttpPost("save")]
        public async Task<IActionResult> SaveAccount([FromBody] AccountMaster model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid data." });
                model.CreatedAt = DateTime.Now;
                model.CreatedBy = 1;
                var message = await _accountService.SaveOrUpdateAsync(model);
                if (message.Contains("Error")) { return Ok(new { success = false, message }); }
                return Ok(new { success = true, message });
            }
            catch (Exception ex)
            {
                string message = "Some error has been occured";
                return Ok(new { success = true, message });
            }
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetAccounts([FromQuery] int? accountId = null)
        {
            var data = await _accountService.GetAccountsAsync(accountId);
            return Ok(new
            {
                success = true,
                count = data.Count,
                data
            });
        }
        [HttpPost]
        [Route("account-group")]
        public async Task<IActionResult> SaveOrUpdateAccountGroup([FromBody] AccountGroupRequest request)
        {
            try
            {
                var (groupId, message) = await _accountService.SaveOrUpdateGroupAsync(request);

                return Ok(new
                {
                    IsSuccess = groupId==0?false:true,
                    GroupId = groupId,
                    Message = message
                });
            }
            catch (Exception ex)
            {
              //  _logger.LogError(ex, "Error saving/updating account group");
                return StatusCode(500, new
                {
                    IsSuccess = false,
                    Message = "Internal error occurred while saving the account group."
                });
            }
        }

        [HttpGet]
        [Route("account-group/list")]
        public async Task<IActionResult> GetAccountGroups()
        {
            try
            {
                var data = await _accountService.GetAccountGroupsAsync();
                return Ok(new
                {
                    IsSuccess = true,
                    Data = data
                });
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error fetching account group list");
                return StatusCode(500, new
                {
                    IsSuccess = false,
                    Message = "An error occurred while fetching account group list."
                });
            }
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetMappings()
        {
            var mappings = await _accountService.GetMappingsAsync();
            return Ok(mappings);
        }
        [HttpPost("add-accounts")]
        public async Task<IActionResult> AddAccountsToGroup([FromBody] AddAccountsToGroupRequest request)
        {
            try
            {
                bool result = await _accountService.AddAccountsToGroupAsync(request);
                return Ok(new { success = result, message = "Accounts added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpGet("groups-with-accounts")]
        public async Task<IActionResult> GetGroupsWithAccounts([FromQuery] int groupId = 0)
        {
            var data = await _accountService.GetAccountGroupsWithAccountsAsync(groupId);
            return Ok(data);
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountRequest request)
        {
            var resultMessage = await _accountService.DeleteAccountAsync(request);

            if (resultMessage.Contains("successfully"))
                return Ok(new { success = true, message = resultMessage });

            return BadRequest(new { success = false, message = resultMessage });
        }

        [HttpPost("manage-accounts")]
        public async Task<IActionResult> AddOrDeleteAccounts([FromBody] AccountGroupActionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.AccountIds))
                return BadRequest(new { success = false, message = "AccountIds cannot be empty." });

            var result = await _accountService.AddOrDeleteAccountsAsync(request);
            return Ok(new { success = true, message = result });
        }


    }

}
