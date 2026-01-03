using Microsoft.AspNetCore.Mvc;
using OnlineChatBackend.DbContexts;
using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Models;
using System.Diagnostics.Eventing.Reader;

namespace OnlineChatBackend.Repositories
{
    public class MessageDBRepository : IMessageRepository
    {
        private AppDbContext _context;

        public MessageDBRepository(AppDbContext context)
        {
            _context = context;
        }
        public void Add(Message message)
        {
            _context.Messages.Add(message);

            var notification = _context.Notifications
                .FirstOrDefault(n => n.DialogId == message.DialogId && n.UserId == message.ToUserId);

            if (notification != null)
            {
                notification.NewNotifications = true;
            }

            _context.SaveChanges();
        }

        public Message? Delete(Message message)
        {
            var foundMessage = _context.Messages.Find(message.Id);
            if (foundMessage != null)
            {
                _context.Messages.Remove(foundMessage);
                _context.SaveChanges();
                return foundMessage;
            }

            return null;
        }

        public List<Message> GetAll(int DialogId, int? UserId)
        {
            List<Message> messages = _context.Messages.Where(x =>  x.DialogId == DialogId).ToList();

            if (UserId.HasValue)
            {
                var notification = _context.Notifications
                    .FirstOrDefault(n => n.DialogId == DialogId && n.UserId == UserId);

                if (notification != null)
                {
                    notification.NewNotifications = false;
                    _context.SaveChanges();
                }
            }

            return messages;
        }

        public Message? GetLastMessage(int DialogId)
        {
            var lastMessage = _context.Messages.Where(x => x.DialogId == DialogId)
                .OrderByDescending(x => x.MessageDateTime)
                .FirstOrDefault();
            return lastMessage;
        }

        public Message? Update(Message message)
        {
            var foundMessage = _context.Messages.Find(message.Id);
            if (foundMessage != null)
            {
                foundMessage.MessageText = message.MessageText;
                foundMessage.Changed = true;
                _context.SaveChanges();
            }

            return foundMessage;
        }
    }
}
