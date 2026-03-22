using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineChatBackend.DbContexts;
using OnlineChatBackend.DTOs;
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

        public IChatsRepository chatsRepository { get; set; }

        private readonly AppDbContext _db;


        private readonly ILogger<ChatHub> _log;

        public ChatHub(
            IMessageRepository messageRepository, 
            ILogger<ChatHub> log,
            IChatsRepository ChatsRepository,
            AppDbContext db)
        {
            this.messageRepository = messageRepository;
            this.chatsRepository = ChatsRepository;
            _log = log;
            _db = db;
        }

        public async Task SendMessage(string message, string chatId, string userId)
        {
            var callerUserIdentifier = Context.UserIdentifier!;
            int fromUserId = int.Parse(callerUserIdentifier);
            int chatIdInt = int.Parse(chatId);
            int toUserId = int.Parse(userId);

            var newMessage = new Message
            {
                MessageText = message,
                ChatId = chatIdInt,
                ToUserId = toUserId,
                FromUserId = fromUserId,
                MessageDateTime = DateTimeOffset.UtcNow
            };

            // сначала сохраняем, чтобы получить реальный Id
            messageRepository.Add(newMessage);

            var dto = new MessageHubDTO
            {
                Id = newMessage.Id,
                ChatId = newMessage.ChatId,
                FromUserId = newMessage.FromUserId,
                ToUserId = newMessage.ToUserId,
                MessageText = newMessage.MessageText,
                MessageDateTime = newMessage.MessageDateTime,
                Changed = newMessage.Changed
            };

            await Clients.User(fromUserId.ToString())
                .SendAsync("MessageCreated", dto);

            await Clients.User(toUserId.ToString())
                .SendAsync("MessageCreated", dto);
        }

        public async Task NewContact(int userId, int newContactId)
        {
            var callerUserId = int.Parse(Context.UserIdentifier!);

            // создаём/находим direct-чат
            var chat = chatsRepository.AddDirectChat(new ChatPostDTO(userId, newContactId), callerUserId);

            // для обоих пользователей ставим NewContact = true
            await MarkNewContactAsync(chat.Id, userId);
            await MarkNewContactAsync(chat.Id, newContactId);

            // просигналить фронту, чтобы тот вызвал refreshContacts()
            await Clients.Caller.SendAsync("NewChat", true);
            await Clients.User(newContactId.ToString()).SendAsync("NewChat", true);
        }

        private async Task MarkNewContactAsync(int chatId, int userId)
        {
            var notif = _db.Notifications
                .FirstOrDefault(n => n.ChatId == chatId && n.UserId == userId);

            if (notif == null)
            {
                notif = new Notification
                {
                    ChatId = chatId,
                    UserId = userId,
                    NewNotifications = false,
                    NewContact = true
                };
                _db.Notifications.Add(notif);
            }
            else
            {
                notif.NewContact = true;
            }

            await _db.SaveChangesAsync();
        }

        public async Task EditMessage(int messageId, string newText)
        {
            var callerUserIdentifier = Context.UserIdentifier!;
            int currentUserId = int.Parse(callerUserIdentifier);

            var updated = messageRepository.Update(messageId, newText, currentUserId);
            if (updated == null)
                throw new HubException("Сообщение не найдено или нет прав на редактирование.");

            var dto = new MessageHubDTO
            {
                Id = updated.Id,
                ChatId = updated.ChatId,
                FromUserId = updated.FromUserId,
                ToUserId = updated.ToUserId,
                MessageText = updated.MessageText,
                MessageDateTime = updated.MessageDateTime,
                Changed = updated.Changed
            };

            await Clients.User(updated.FromUserId.ToString())
                .SendAsync("MessageEdited", dto);

            await Clients.User(updated.ToUserId.ToString())
                .SendAsync("MessageEdited", dto);
        }

        public async Task DeleteMessage(int messageId)
        {
            var callerUserIdentifier = Context.UserIdentifier!;
            int currentUserId = int.Parse(callerUserIdentifier);

            var deleted = messageRepository.Delete(messageId, currentUserId);
            if (deleted == null)
                throw new HubException("Сообщение не найдено или нет прав на удаление.");

            await Clients.User(deleted.FromUserId.ToString())
                .SendAsync("MessageDeleted", deleted.Id);

            await Clients.User(deleted.ToUserId.ToString())
                .SendAsync("MessageDeleted", deleted.Id);
        }


    }
}
