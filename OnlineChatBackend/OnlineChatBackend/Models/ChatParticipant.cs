using System.ComponentModel.DataAnnotations;

namespace OnlineChatBackend.Models
{
    public class ChatParticipant
    {
        [Key]
        public int Id { get; set; }

        public int ChatId { get; set; }
        public int UserId { get; set; }

        // Права/роль, настройки и т.п.
        public bool IsAdmin { get; set; } = false;
        public DateTimeOffset JoinedAt { get; set; }

        public Chat? Chat { get; set; }
        public Contact? User { get; set; }
    }
}
