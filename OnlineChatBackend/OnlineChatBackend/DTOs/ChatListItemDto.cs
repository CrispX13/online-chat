using OnlineChatBackend.Models;

namespace OnlineChatBackend.DTOs
{
    public class ChatListItemDto
    {
        public int ChatId { get; set; }
        public ChatType Type { get; set; } // Direct / Group
        public string Title { get; set; }  // имя собеседника или имя группы
        public bool NewNotifications { get; set; }
        public bool NewContact { get; set; }
    }
}
