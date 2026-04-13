namespace OnlineChatBackend.DTOs
{
    public class SendSearchSelectionDTO
    {
        public string ChatId { get; set; } = "";
        public List<int> SelectedIndexes { get; set; } = new();
    }
}
