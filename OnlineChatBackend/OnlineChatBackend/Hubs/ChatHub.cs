using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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

        public async Task SendMessage(string message, string chatId)
        {
            _log.LogInformation("SendMessage: raw chatId = {ChatId}, userId = {UserId}", chatId, Context.UserIdentifier);
            _log.LogInformation("UserIdentifier = {UserIdentifier}", Context.UserIdentifier);

            foreach (var claim in Context.User?.Claims ?? [])
            {
                _log.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
            }

            int fromUserId = int.Parse(Context.UserIdentifier!);
            int chatIdInt = int.Parse(chatId);

            var chat = await _db.Chats
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == chatIdInt);

            if (chat == null)
                throw new HubException("Чат не найден.");

            _log.LogInformation("Chat participants: {Participants}",
                string.Join(", ", chat.Participants.Select(p => p.UserId)));

            var isMember = chat.Participants.Any(p => p.UserId == fromUserId);
            if (!isMember)
                throw new HubException("Вы не являетесь участником этого чата.");

            // toUserId только для Direct-чата
            int? toUserId = null;
            if (chat.Type == ChatType.Direct)
            {
                toUserId = chat.Participants
                    .Select(p => p.UserId)
                    .FirstOrDefault(id => id != fromUserId);

                if (toUserId == 0) // FirstOrDefault вернул дефолт
                    throw new HubException("Второй участник Direct-чата не найден.");
            }

            var newMessage = new Message
            {
                MessageText = message,
                ChatId = chatIdInt,
                ToUserId = toUserId,       // null для группового — OK
                FromUserId = fromUserId,
                MessageDateTime = DateTimeOffset.UtcNow
            };

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

            // Рассылаем всем участникам (работает и для Direct, и для Group)
            var userIds = chat.Participants.Select(p => p.UserId).Distinct();
            foreach (var uid in userIds)
            {
                await Clients.User(uid.ToString()).SendAsync("MessageCreated", dto);
            }
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
            int currentUserId = int.Parse(Context.UserIdentifier!);

            var updated = messageRepository.Update(messageId, newText, currentUserId);
            if (updated == null)
                throw new HubException("Сообщение не найдено или нет прав.");

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

            // Получаем всех участников чата, а не только FromUserId/ToUserId
            var chat = await _db.Chats
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == updated.ChatId);

            if (chat != null)
            {
                foreach (var uid in chat.Participants.Select(p => p.UserId).Distinct())
                    await Clients.User(uid.ToString()).SendAsync("MessageEdited", dto);
            }
        }

        public async Task DeleteMessage(int messageId)
        {
            int currentUserId = int.Parse(Context.UserIdentifier!);

            var deleted = messageRepository.Delete(messageId, currentUserId);
            if (deleted == null)
                throw new HubException("Сообщение не найдено или нет прав.");

            var chat = await _db.Chats
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == deleted.ChatId);

            if (chat != null)
            {
                foreach (var uid in chat.Participants.Select(p => p.UserId).Distinct())
                    await Clients.User(uid.ToString()).SendAsync("MessageDeleted", deleted.Id);
            }
        }

    }
}
