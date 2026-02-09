using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineChatBackend.Models
{
    public class Contact
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        [MinLength(10)]
        public string? Name { get; set; }

        public string PasswordHash { get; set; }

        public string? AvatarUrl { get; set; }  = "avatars/default.png";

        public List<Dialog> DialogsAsFirstUser { get; set; } = new();

        public List<Dialog> DialogsAsSecondUser { get; set; } = new();

        public List<Notification> Notifications { get; set; } = new();

        //[Required]
        //public string TagName { get; set; }
    }
}
