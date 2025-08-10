using Microsoft.AspNetCore.Mvc;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.Service.Interfaces;

namespace OTS.MobileAccountingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestVM request)
        {
            var result = await _authService.LoginAsync(request);
            if (result == null)
            {
                return Unauthorized();
            }

            return Ok(result);
        }
    }
}
