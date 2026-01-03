using OnlineChatBackend.Models;

namespace OnlineChatBackend.DTOs
{
    public sealed class ContactWithStatusDto
    {
        public Contact Contact { get; set; }
        public bool NewNotifications { get; set; }
    }
}
