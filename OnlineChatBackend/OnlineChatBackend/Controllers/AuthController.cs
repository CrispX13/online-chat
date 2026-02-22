using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineChatBackend.DTOs;
using OnlineChatBackend.Services;
using System.Security.Claims;

namespace OnlineChatBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly AccountService _accountService;

        public AuthController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUserRequest request)
        {
            _accountService.Register(request);
            return NoContent();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var result = _accountService.Login(loginRequest.UserName, loginRequest.Password);

            // result.token — это JWT из JwtService.GenerateToken(account)
            var jwt = result.token;

            var expires = DateTime.UtcNow.AddDays(7);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = expires
            };

            Response.Cookies.Append("accessToken", jwt, cookieOptions);

            return Ok(new { result });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("accessToken");
            return Ok();
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var name = User.Identity?.Name;

            if (id == null)
                return Unauthorized();

            // достаём токен из cookie
            var jwt = Request.Cookies["accessToken"];

            return Ok(new
            {
                id,
                userName = name,
                token = jwt   // <<< добавляем токен
            });
        }
    }
}
