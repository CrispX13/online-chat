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


        [HttpGet("direct")]
        public IActionResult GetChat([FromQuery] ChatPostDTO chat)
        {
            chat.Normalize();
            int currentUserId = int.Parse(User.FindFirst("id")!.Value);
            var chatKey = _chatsRepository.GetDirectChat(chat, currentUserId);

            if (chatKey == null)
            {
                return BadRequest();
            }
            else
                return Ok(new { chatKey });
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
    }
}
