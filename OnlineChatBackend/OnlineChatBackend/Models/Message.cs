using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineChatBackend.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public int ToUserId { get; set; }

        public int FromUserId { get; set; }

        public int DialogId { get; set; }
        [Required]
        public string MessageText { get; set; }

        public DateTimeOffset MessageDateTime { get; set; }

        public bool Changed { get; set; } = false;

        public Dialog Dialog { get; set; }
 
    }
}
