using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineChatBackend.DbContexts;
using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Models;

namespace OnlineChatBackend.Repositories
{
    public class MessageDBRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageDBRepository(AppDbContext context)
        {
            _context = context;
        }

        // Добавить сообщение
        public void Add(Message message)
        {
            // проверяем, что отправитель участник чата
            var chat = _context.Chats
                .Include(c => c.Participants)
                .FirstOrDefault(c =>
                    c.Id == message.ChatId &&
                    (
                        // Direct: через First/SecondUserId
                        (c.Type == ChatType.Direct &&
                         (c.FirstUserId == message.FromUserId || c.SecondUserId == message.FromUserId))
                        ||
                        // Group: через участников
                        (c.Type == ChatType.Group &&
                         c.Participants.Any(p => p.UserId == message.FromUserId))
                    ));

            if (chat == null)
                throw new UnauthorizedAccessException("Нет доступа к чату.");

            _context.Messages.Add(message);

            // уведомление только для ToUserId
            var notification = _context.Notifications
                .FirstOrDefault(n => n.ChatId == message.ChatId &&
                                     n.UserId == message.ToUserId);

            if (notification == null)
            {
                notification = new Notification
                {
                    ChatId = message.ChatId,
                    UserId = message.ToUserId,
                    NewNotifications = true,
                    NewContact = false
                };
                _context.Notifications.Add(notification);
            }
            else
            {
                notification.NewNotifications = true;
            }

            _context.SaveChanges();
        }

        // Получить все сообщения чата
        public List<Message> GetAll(int chatId, int currentUserId)
        {
            var chat = _context.Chats
                .Include(c => c.Participants)
                .FirstOrDefault(c =>
                    c.Id == chatId &&
                    (
                        (c.Type == ChatType.Direct &&
                         (c.FirstUserId == currentUserId || c.SecondUserId == currentUserId))
                        ||
                        (c.Type == ChatType.Group &&
                         c.Participants.Any(p => p.UserId == currentUserId))
                    ));

            if (chat == null)
                return new List<Message>();

            var messages = _context.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.MessageDateTime)
                .ToList();

            var notification = _context.Notifications
                .FirstOrDefault(n => n.ChatId == chatId &&
                                     n.UserId == currentUserId);

            if (notification != null)
            {
                notification.NewNotifications = false;
                notification.NewContact = false;
                _context.SaveChanges();
            }

            return messages;
        }

        // Последнее сообщение в чате
        public Message? GetLastMessage(int chatId, int currentUserId)
        {
            var chat = _context.Chats
                .Include(c => c.Participants)
                .FirstOrDefault(c =>
                    c.Id == chatId &&
                    (
                        (c.Type == ChatType.Direct &&
                         (c.FirstUserId == currentUserId || c.SecondUserId == currentUserId))
                        ||
                        (c.Type == ChatType.Group &&
                         c.Participants.Any(p => p.UserId == currentUserId))
                    ));

            if (chat == null)
                return null;

            return _context.Messages
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.MessageDateTime)
                .FirstOrDefault();
        }

        // Обновление сообщения (редактировать может только отправитель и участник чата)
        public Message? Update(int messageId, string newText, int currentUserId)
        {
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
            var message = _context.Messages
                .Include(m => m.Chat)
                    .ThenInclude(c => c.Participants)
                .FirstOrDefault(m =>
                    m.Id == messageId &&
                    m.FromUserId == currentUserId &&
                    (
                        (m.Chat.Type == ChatType.Direct &&
                         (m.Chat.FirstUserId == currentUserId || m.Chat.SecondUserId == currentUserId))
                        ||
                        (m.Chat.Type == ChatType.Group &&
                         m.Chat.Participants.Any(p => p.UserId == currentUserId))
                    ));
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.

            if (message == null)
                return null;

            message.MessageText = newText;
            message.Changed = true;
            _context.SaveChanges();
            return message;
        }

        // Удаление сообщения
        public Message? Delete(int messageId, int currentUserId)
        {
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
            var message = _context.Messages
                .Include(m => m.Chat)
                    .ThenInclude(c => c.Participants)
                .FirstOrDefault(m =>
                    m.Id == messageId &&
                    m.FromUserId == currentUserId &&
                    (
                        (m.Chat.Type == ChatType.Direct &&
                         (m.Chat.FirstUserId == currentUserId || m.Chat.SecondUserId == currentUserId))
                        ||
                        (m.Chat.Type == ChatType.Group &&
                         m.Chat.Participants.Any(p => p.UserId == currentUserId))
                    ));
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.

            if (message == null)
                return null;

            _context.Messages.Remove(message);
            _context.SaveChanges();
            return message;
        }
    }
}