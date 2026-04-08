namespace OnlineChatBackend.DTOs
{
    public class ChatParticipantDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string? AvatarUrl { get; set; }
    }
}
