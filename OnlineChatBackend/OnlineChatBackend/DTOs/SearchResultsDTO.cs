namespace OnlineChatBackend.DTOs
{
    public class SearchResultsDTO
    {
        public int ChatId { get; set; }
        public string Query { get; set; } = "";
        public List<SearchResultItemDTO> Results { get; set; } = new();
    }
}
