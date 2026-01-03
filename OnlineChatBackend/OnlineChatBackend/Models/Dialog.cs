using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineChatBackend.Models
{
    public class Dialog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int FirstUserId { get; set; }
        [Required]
        public int SecondUserId{ get; set; }

        public Dialog() { }
        public Dialog(int FirstUserId, int SecondUserId)
        {
            this.FirstUserId =  FirstUserId>SecondUserId?SecondUserId:FirstUserId;
            this.SecondUserId = FirstUserId < SecondUserId ? SecondUserId : FirstUserId;
        }

        public List<Message> Messages{ get; set; } = new List<Message>();

        public Contact FirstUser { get; set; }

        public Contact SecondUser { get; set; }

        public List<Notification> Notifications { get; set; } = new();
    }
}
