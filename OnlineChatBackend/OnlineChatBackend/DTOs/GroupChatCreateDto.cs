namespace OnlineChatBackend.DTOs
{
    public class GroupChatCreateDto
    {
        public string Name { get; set; }
        public List<int> ParticipantIds { get; set; } = new();
    }
}
