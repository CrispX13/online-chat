namespace OnlineChatBackend.DTOs
{
    public class SearchResultItemDTO
    {
        public int Index { get; set; }
        public string Title { get; set; } = "";
        public string Url { get; set; } = "";
        public string Snippet { get; set; } = "";
    }
}
