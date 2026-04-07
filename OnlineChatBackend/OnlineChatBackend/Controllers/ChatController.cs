using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineChatBackend.DTOs;
using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Models;

namespace OnlineChatBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : Controller
    {
        private IChatsRepository _chatsRepository;

        public IMessageRepository _messageRepository { get; set; }

        public ChatController(IChatsRepository chatsRepository, IMessageRepository messageRepository)
        {
            _chatsRepository = chatsRepository;
            _messageRepository = messageRepository;
        }


        [HttpPost("new")]
        public IActionResult NewChat([FromBody] ChatPostDTO chat)
        {
            int currentUserId = int.Parse(User.FindFirst("id")!.Value);
            var Key = _chatsRepository.AddDirectChat(chat, currentUserId);

            if (Key == null)
                return NotFound();

            return Ok(Key);
        }


        [HttpPost("{chatId}/messages")]
        public IActionResult AddMessage([FromBody] MessageDTO message)
        {
            Message newMessage = new Message
            {
                ChatId = message.ChatId,
                MessageText = message.TextMessage,
                MessageDateTime = DateTimeOffset.UtcNow
            };
            _messageRepository.Add(newMessage);
            return Ok(newMessage);
        }

        [HttpGet("{chatId:int}/messages")]
        public IActionResult ReadAll(int chatId)
        {
            int currentUserId = int.Parse(User.FindFirst("id")!.Value);
            var messages = _messageRepository.GetAll(chatId, currentUserId);
            return Ok(messages);
        }

        // POST api/Chat/group
        [HttpPost("group")]
        public IActionResult CreateGroup([FromBody] GroupChatCreateDto dto)
        {
            int currentUserId = int.Parse(User.FindFirst("id")!.Value);

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Имя группы не может быть пустым.");

            // владелец всегда внутри ParticipantIds не обязателен – мы добавляем сами
            var chat = _chatsRepository.CreateGroupChat(dto.Name, currentUserId, dto.ParticipantIds);

            return Ok(chat); // можно вернуть DTO, если не хочешь светить Participants целиком
        }

        // GET api/Chat/{chatId}
        [HttpGet("{chatId:int}")]
        public IActionResult GetChatById(int chatId)
        {
            int currentUserId = int.Parse(User.FindFirst("id")!.Value);

            var chat = _chatsRepository.GetChatById(chatId, currentUserId);
            if (chat == null)
                return NotFound();

            return Ok(chat);
        }
    }
}
