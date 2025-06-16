using Factory.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Factory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AuthService _authService;

        public AdminController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var (success, token) = _authService.Authenticate(model.Username, model.Password);

            if (!success)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new { token });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var username = User.Identity.Name;
            _authService.Logout(username);
            return Ok(new { message = "Logged out successfully" });
        }

        [ApiController]
        [Route("api/[controller]")]
        public class TestController : ControllerBase
        {
            [HttpGet("hash")]
            public IActionResult HashPassword([FromQuery] string password)
            {
                return Ok(new
                {
                    password = password,
                    hash = BCrypt.Net.BCrypt.HashPassword(password)
                });
            }
        }

    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
