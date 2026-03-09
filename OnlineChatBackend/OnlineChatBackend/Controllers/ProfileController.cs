using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineChatBackend.DTOs;
using OnlineChatBackend.Repositories;
using OnlineChatBackend.Services;

namespace OnlineChatBackend.Controllers
{
    [Route("api/profile")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly ContactsDBRepository _repo;
        private readonly IWebHostEnvironment _env;

        public ProfileController(AccountService accountService, ContactsDBRepository repo, IWebHostEnvironment env)
        {
            _accountService = accountService;
            _repo = repo;
            _env = env;
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst("id")
                          ?? User.FindFirst("userId");

            if (idClaim == null)
                throw new UnauthorizedAccessException("Не найден claim с Id пользователя.");

            return int.Parse(idClaim.Value);
        }

        // Сменить пароль ТЕКУЩЕГО пользователя
        [HttpPut("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordDto dto)
        {
            int currentUserId = GetCurrentUserId();

            if (_accountService.ChangePassword(currentUserId, dto.LastPassword, dto.NewPassword))
            {
                return Ok(new
                {
                    message = "Пароль успешно изменен"
                });
            }

            return BadRequest(new
            {
                message = "Во время изменения пароля произошла ошибка"
            });
        }

        // Сменить имя ТЕКУЩЕГО пользователя
        [HttpPut("change-name")]
        public IActionResult ChangeName([FromBody] string name)
        {
            int currentUserId = GetCurrentUserId();

            if (_accountService.ChangeName(currentUserId, name))
            {
                return Ok(new
                {
                    message = "Имя успешно изменено"
                });
            }

            return BadRequest(new
            {
                message = "Во время изменения имени произошла ошибка"
            });
        }

        // Загрузить аватар ТЕКУЩЕГО пользователя
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не передан");

            int currentUserId = GetCurrentUserId();

            var result = await _repo.ChangeAvatarAsync(currentUserId, file);
            if (!result) return BadRequest();

            return Ok();
        }

        // Удалить аватар ТЕКУЩЕГО пользователя (сбросить на дефолтный)
        [HttpDelete("avatar")]
        public IActionResult DeleteAvatar()
        {
            int currentUserId = GetCurrentUserId();

            var result = _repo.DeleteAvatar(currentUserId);
            if (!result) return NotFound();

            return Ok();
        }

        // Получить картинку аватара по ID контакта (можно оставлять публичным)
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

            var contentType = "image/png"; // можно определить по расширению
            return PhysicalFile(fullPath, contentType);
        }
    }
}
