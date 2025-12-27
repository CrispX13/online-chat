using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineChatBackend.Models
{
    public class Notification
    {
        public int DialogId { get; set; }
        public int UserId { get; set; }

        public bool NewNotifications { get; set; }
    }
}
