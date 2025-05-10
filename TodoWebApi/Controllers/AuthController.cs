using Microsoft.AspNetCore.Mvc;
using TodoWebApi.Services;
using TodoWebApi.DTOs;

namespace TodoWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            if (await _authService.UserExist(dto.Username))
            {
                return BadRequest("Username already taken.");
            }

            var user = await _authService.Register(dto);
            return Ok(new { user.Id, user.Username });
        }
    }
}
