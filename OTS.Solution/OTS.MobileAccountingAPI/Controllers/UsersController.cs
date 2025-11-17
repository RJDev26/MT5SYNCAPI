using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.Service.Interfaces;

namespace OTS.MobileAccountingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserListItemVM>>> GetUsers([FromQuery] string? role, [FromQuery] bool? isActive)
        {
            var users = await _userService.GetUserListAsync(role, isActive);
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelStateErrorsResponse("Invalid user data."));
            }

            var response = await _userService.CreateUserAsync(request);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] EditUserRequestVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelStateErrorsResponse("Invalid user data."));
            }

            var response = await _userService.UpdateUserAsync(id, request);
            if (response.IsNotFound)
            {
                return NotFound(response);
            }

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] ChangePasswordRequestVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelStateErrorsResponse("Invalid password change request."));
            }

            var response = await _userService.ChangePasswordAsync(id, request);
            if (response.IsNotFound)
            {
                return NotFound(response);
            }

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(string id, [FromBody] ResetPasswordRequestVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelStateErrorsResponse("Invalid password reset request."));
            }

            var response = await _userService.ResetPasswordAsync(id, request);
            if (response.IsNotFound)
            {
                return NotFound(response);
            }

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("{userId:int}/manager-mapping")]
        public async Task<IActionResult> SetUserManagerMapping(int userId, [FromBody] UserManagerMappingRequestVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelStateErrorsResponse("Invalid manager mapping data."));
            }

            request.UserId = userId;
            var response = await _userService.SetUserManagerMappingAsync(request);
            if (response.IsNotFound)
            {
                return NotFound(response);
            }

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{userId:int}/managers")]
        public async Task<ActionResult<IEnumerable<UserManagerVM>>> GetUserManagers(int userId)
        {
            var managers = await _userService.GetManagersByUserIdAsync(userId);
            return Ok(managers);
        }

        private UserResponseVM ModelStateErrorsResponse(string message)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message : e.ErrorMessage)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Cast<string>()
                .ToList();

            return new UserResponseVM
            {
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
        }
    }
}
