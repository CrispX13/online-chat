using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineChatBackend.Models
{
    public class Notification
    {
        public int ChatId { get; set; }
        public int UserId { get; set; }
        public bool NewNotifications { get; set; }

        public bool NewContact {  get; set; }

        public Chat? Chat { get; set; }
        public Contact? User { get; set; }
    }
}
