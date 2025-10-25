using Microsoft.AspNetCore.Mvc;
using OnlineChatBackend.DTOs;
using OnlineChatBackend.Services;

namespace OnlineChatBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(AccountService accountService) : Controller
    {
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUserRequest request)
        {
            accountService.Register(request);
            return NoContent();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var result = accountService.Login(loginRequest.UserName, loginRequest.Password);
            return Ok(new { result });
        }
    }
}
