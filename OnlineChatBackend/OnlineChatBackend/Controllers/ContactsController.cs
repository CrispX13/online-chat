using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineChatBackend.DbContexts;
using OnlineChatBackend.DTOs;
using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Models;
using System.Security.Claims;

namespace OnlineChatBackend.Controllers
{
    [ApiController]
    [Route("api/contacts")]
    [Authorize]
    public class ContactsController : ControllerBase
    {
        private readonly IContactsRepository _contactsRepository;

        private readonly AppDbContext _context;

        public ContactsController(IContactsRepository contactsRepository, AppDbContext context)
        {
            _contactsRepository = contactsRepository;
            _context = context;
        }

        private int GetCurrentUserId()
        {
            // Подставь сюда тот claim, который реально используешь для Id
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst("id")
                          ?? User.FindFirst("userId");

            if (idClaim == null)
                throw new UnauthorizedAccessException("Не найден claim с Id пользователя.");

            return int.Parse(idClaim.Value);
        }

        // Профиль текущего пользователя
        [HttpGet("me")]
        public ActionResult<ContactDTO> GetMe()
        {
            int currentUserId = GetCurrentUserId();

            var contact = _contactsRepository.GetCurrentUser(currentUserId);
            if (contact == null)
                return NotFound();

            var dto = new ContactDTO
            {
                Id = contact.Id,
                Name = contact.Name
            };

            return Ok(dto);
        }

        // Список контактов/диалогов текущего пользователя
        [HttpGet("me/list")]
        public ActionResult<IEnumerable<ContactWithStatusDto>> GetMyContacts()
        {
            int currentUserId = GetCurrentUserId();

            var contacts = _contactsRepository.FindAllForUser(currentUserId);
            return Ok(contacts);
        }

        // Изменить имя текущего пользователя
        [HttpPut("me/name")]
        public IActionResult ChangeMyName([FromBody] ChangeNameDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NewName))
                return BadRequest("Имя не может быть пустым");

            int currentUserId = int.Parse(User.FindFirst("id")!.Value);
            var ok = _contactsRepository.ChangeUserName(currentUserId, dto.NewName);
            if (!ok) return BadRequest();

            return Ok();
        }

        // Изменить пароль текущего пользователя
        [HttpPut("me/password")]
        public IActionResult ChangeMyPassword([FromBody] ChangePasswordDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest("Пароль не может быть пустым.");

            int currentUserId = GetCurrentUserId();

            var result = _contactsRepository.ChangePassword(currentUserId, dto.NewPassword);
            if (!result)
                return NotFound();

            return NoContent();
        }

        // Получить путь к аватару текущего пользователя
        [HttpGet("me/avatar")]
        public ActionResult<string> GetMyAvatar()
        {
            int currentUserId = GetCurrentUserId();

            var path = _contactsRepository.GetAvatarPath(currentUserId);
            if (path == null)
                return NotFound();

            return Ok(path);
        }

        // Загрузить/сменить аватар текущего пользователя
        [HttpPost("me/avatar")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ChangeMyAvatar([FromForm] AvatarUploadDto dto)
        {
            if (dto.Avatar == null || dto.Avatar.Length == 0)
                return BadRequest("Файл аватара не передан.");

            int currentUserId = GetCurrentUserId();

            var result = await _contactsRepository.ChangeAvatarAsync(currentUserId, dto.Avatar);
            if (!result)
                return NotFound();

            return NoContent();
        }


        // Сбросить аватар на дефолтный
        [HttpDelete("me/avatar")]
        public IActionResult ResetMyAvatar()
        {
            int currentUserId = GetCurrentUserId();

            var result = _contactsRepository.SetDefaultAvatar(currentUserId);
            if (!result)
                return NotFound();

            return NoContent();
        }

        // Поиск контактов по части имени (опционально – зависит от того, кому это нужно)
        [HttpPost("search")]
        public ActionResult<IEnumerable<Contact>> Search([FromBody] string partOfName)
        {
            if (string.IsNullOrWhiteSpace(partOfName))
                return BadRequest("Строка поиска не может быть пустой.");

            var result = _contactsRepository.Search(partOfName) ?? Enumerable.Empty<Contact>();
            return Ok(result);
        }

        [HttpGet("me/chats")]
        public IActionResult GetMyChats()
        {
            int currentUserId = int.Parse(User.FindFirst("id")!.Value);
            var chats = _contactsRepository.GetChatsForUser(currentUserId);
            return Ok(chats);
        }

        [HttpGet("me/groups")]
        public ActionResult<IEnumerable<GroupChatDto>> GetMyGroupChats()
        {
            int currentUserId = GetCurrentUserId();

            var groups = _context.Chats
                .Where(c => c.Type == ChatType.Group &&
                            c.Participants.Any(p => p.UserId == currentUserId))
                .Select(c => new GroupChatDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    MembersCount = c.Participants.Count,
                    NewNotifications = _context.Notifications
                        .Any(n => n.ChatId == c.Id &&
                                  n.UserId == currentUserId &&
                                  n.NewNotifications)
                })
                .ToList();

            return Ok(groups);
        }
    }

    public class ChangeNameDto
    {
        public string NewName { get; set; } = string.Empty;
    }
}
