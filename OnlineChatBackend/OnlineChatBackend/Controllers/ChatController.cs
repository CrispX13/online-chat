using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineChatBackend.DTOs;
using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Models;
using OnlineChatBackend.Repositories;

namespace OnlineChatBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : Controller
    {
        public IMessageRepository messageRepository{ get; set; }

        public ChatController(IMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        [HttpPost]
        public IActionResult AddMessage([FromBody] MessageDTO message)
        {
            Message newMessage = new Message {
                DialogId = message.DialogId,
                MessageText = message.TextMessage,
                MessageDateTime = DateTimeOffset.UtcNow
            };
            messageRepository.Add(newMessage);
            return Ok(newMessage);
        }

        [HttpGet]
        public IActionResult ReadAll(int DialogKey)
        {
            List<Message> messages = messageRepository.GetAll(DialogKey);
            return Ok(messages);
        }
    }
}
