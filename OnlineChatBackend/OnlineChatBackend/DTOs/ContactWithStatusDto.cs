using OnlineChatBackend.Models;

namespace OnlineChatBackend.DTOs
{
    public sealed class ContactWithStatusDto
    {
        public required Contact Contact { get; set; }
        public bool NewNotifications { get; set; }
        public int ChatId { get; set; }
        public bool NewContact { get; set; } = false;
    }
}
