using OnlineChatBackend.Models;
using System.ComponentModel.DataAnnotations;

public class Contact
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    [MinLength(10)]
    public string Name { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; } = "avatars/default.png";

    public List<Notification> Notifications { get; set; } = new();
    public List<ChatParticipant> ChatParticipants { get; set; } = new();
}