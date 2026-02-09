using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineChatBackend.DTOs;
using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Repositories;
using OnlineChatBackend.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineChatBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController(AccountService accountService, ContactsDBRepository _repo, IWebHostEnvironment _env) : ControllerBase
    {

        [HttpPut("change-password/{id}")]
        public IActionResult ChangePassword(int id,[FromBody] ChangePasswordDto dto)
        {
            if (accountService.ChangePassword(id, dto.LastPassword, dto.NewPassword))
            {
                return Ok(new
                {
                    message = "Пароль успешно изменен"
                });
            }
            else
            {
                return BadRequest(new
                {
                    message = "Во время изенения пароля произошла ошибка"
                });
            }
        }

        [HttpPut("change-name/{id}")]
        public IActionResult ChangeName(int id, [FromBody] string Name)
        {
            if (accountService.ChangeName(id, Name))
            {
                return Ok(new
                {
                    message = "Пароль успешно изменен"
                });
            }
            else
            {
                return BadRequest(new
                {
                    message = "Во время изенения пароля произошла ошибка"
                });
            }
        }

        [HttpPost("{id}/avatar")]
        public async Task<IActionResult> UploadAvatar(int id, IFormFile file)
        {
            var result = await _repo.ChangeAvatarAsync(id, file);
            if (!result) return BadRequest();
            return Ok();
        }

        [HttpDelete("{id}/avatar")]
        public IActionResult DeleteAvatar(int id)
        {
            var result = _repo.DeleteAvatar(id);
            if (!result) return NotFound();
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("{id}/avatar")]
        public IActionResult GetAvatar(int id)
        {
            var contact = _repo.GetContact(id);
            if (contact == null)
                return NotFound();

            var relativePath = string.IsNullOrEmpty(contact.AvatarUrl)
                ? "avatars/default.png"
                : contact.AvatarUrl;

            var fullPath = Path.Combine(_env.WebRootPath, relativePath);

            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            var contentType = "image/png";
            return PhysicalFile(fullPath, contentType);
        }
    }
}
