namespace OnlineChatBackend.DTOs
{
    public class MessageHubDTO
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public int? FromUserId { get; set; }
        public int? ToUserId { get; set; }
        public string? MessageText { get; set; }
        public DateTimeOffset MessageDateTime { get; set; }
        public bool Changed { get; set; }
    }
}
