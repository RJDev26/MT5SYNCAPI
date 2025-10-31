using System.Linq;
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
