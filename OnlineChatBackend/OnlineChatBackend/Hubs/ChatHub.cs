using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Models;
using SignalRSwaggerGen.Attributes;

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

        public async Task JoinDialog(string DialogId)
        {
            _log.LogInformation("Connected: {ConnId}, Authenticated: {Auth}, Name: {Name}",
            Context.ConnectionId, Context.User?.Identity?.IsAuthenticated, Context.User?.Identity?.Name);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"dialog:{DialogId}");
        }

        public async Task LeaveDialog(string DialogId)
        {
            _log.LogInformation("JoinDialog called. Conn={ConnId}, DialogId={DialogId}",
            Context.ConnectionId, DialogId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"dialog:{DialogId}");
        }

        public async Task SendMessage(string Message,string DialogId, string UserId)
        {
            Message newMessage = new Message
            {
                MessageText = Message,
                DialogId = Int32.Parse(DialogId),
                MessageDateTime = DateTime.UtcNow,
                UserId = Int32.Parse(UserId)
            };
            await Clients.Group($"dialog:{DialogId}").SendAsync("MessageCreated", newMessage);
            messageRepository.Add(newMessage);
        }

        public async Task SendNotification(string fromContactId,string toContactId)
        {
            await Clients.Group(toContactId).SendAsync("NewNotifications", fromContactId);
        }
    }
}
