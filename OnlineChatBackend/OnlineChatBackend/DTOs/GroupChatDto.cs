namespace OnlineChatBackend.DTOs
{
    public class GroupChatDto
    {
        public int Id { get; set; }        // Chat.Id (chatId)
        public string Name { get; set; } = null!;
        public int MembersCount { get; set; }
        public bool NewNotifications { get; set; }
    }
}
