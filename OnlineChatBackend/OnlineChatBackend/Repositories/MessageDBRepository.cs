using Microsoft.AspNetCore.Mvc;
using OnlineChatBackend.DbContexts;
using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Models;
using System.Diagnostics.Eventing.Reader;

namespace OnlineChatBackend.Repositories
{
    public class MessageDBRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageDBRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Message message)
        {
            // проверяем, что отправитель участник диалога
            var dialog = _context.Dialogs
                .FirstOrDefault(d => d.Id == message.DialogId &&
                                     (d.FirstUserId == message.FromUserId ||
                                      d.SecondUserId == message.FromUserId));

            if (dialog == null)
                throw new UnauthorizedAccessException("Нет доступа к диалогу.");

            _context.Messages.Add(message);

            var notification = _context.Notifications
                .FirstOrDefault(n => n.DialogId == message.DialogId &&
                                     n.UserId == message.ToUserId);

            if (notification == null)
            {
                notification = new Notification
                {
                    DialogId = message.DialogId,
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

        public List<Message> GetAll(int dialogId, int currentUserId)
        {
            var dialog = _context.Dialogs
                .FirstOrDefault(d => d.Id == dialogId &&
                                     (d.FirstUserId == currentUserId ||
                                      d.SecondUserId == currentUserId));

            if (dialog == null)
                return new List<Message>();

            var messages = _context.Messages
                .Where(m => m.DialogId == dialogId)
                .OrderBy(m => m.MessageDateTime)
                .ToList();

            var notification = _context.Notifications
                .FirstOrDefault(n => n.DialogId == dialogId &&
                                     n.UserId == currentUserId);

            if (notification != null)
            {
                notification.NewNotifications = false;
                notification.NewContact = false;
                _context.SaveChanges();
            }

            return messages;
        }

        public Message? GetLastMessage(int dialogId, int currentUserId)
        {
            var dialog = _context.Dialogs
                .FirstOrDefault(d => d.Id == dialogId &&
                                     (d.FirstUserId == currentUserId ||
                                      d.SecondUserId == currentUserId));

            if (dialog == null)
                return null;

            return _context.Messages
                .Where(m => m.DialogId == dialogId)
                .OrderByDescending(m => m.MessageDateTime)
                .FirstOrDefault();
        }

        public Message? Update(int messageId, string newText, int currentUserId)
        {
            var message = _context.Messages
                .FirstOrDefault(m => m.Id == messageId &&
                                     m.FromUserId == currentUserId &&
                                     (m.Dialog.FirstUserId == currentUserId ||
                                      m.Dialog.SecondUserId == currentUserId));

            if (message == null)
                return null;

            message.MessageText = newText;
            message.Changed = true;
            _context.SaveChanges();
            return message;
        }

        public Message? Delete(int messageId, int currentUserId)
        {
            var message = _context.Messages
                .FirstOrDefault(m => m.Id == messageId &&
                                     m.FromUserId == currentUserId &&
                                     (m.Dialog.FirstUserId == currentUserId ||
                                      m.Dialog.SecondUserId == currentUserId));

            if (message == null)
                return null;

            _context.Messages.Remove(message);
            _context.SaveChanges();
            return message;
        }

    }
}

