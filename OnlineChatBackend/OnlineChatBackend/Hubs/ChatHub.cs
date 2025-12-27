using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Models;
using SignalRSwaggerGen.Attributes;
using System.Collections.Concurrent;

namespace OnlineChatBackend.Hubs
{
    [Authorize]
    [SignalRHub]
    public class ChatHub:Hub
    {
        public IMessageRepository messageRepository { get; set; }

        private readonly ILogger<ChatHub> _log;

        public ChatHub(IMessageRepository messageRepository, ILogger<ChatHub> log)
        {
            this.messageRepository = messageRepository;
            _log = log;
        }

        public async Task SendMessage(string Message,string DialogId, string UserId)
        {

            var callerUserIdentifier = Context.UserIdentifier;

            Message newMessage = new Message
            {
                MessageText = Message,
                DialogId = Int32.Parse(DialogId),
                ToUserId = Int32.Parse(UserId),
                FromUserId = Int32.Parse(callerUserIdentifier),
                MessageDateTime = DateTime.UtcNow
            };

            await Clients.Caller.SendAsync("MessageCreated", newMessage);
            await Clients.User(UserId).SendAsync("MessageCreated", newMessage);

            messageRepository.Add(newMessage);

        }
 
    }
}
