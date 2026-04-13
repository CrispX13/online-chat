using OnlineChatBackend.DTOs;

namespace OnlineChatBackend.Models
{
    public class SearchSession
    {
        public string Query { get; set; } = "";
        public List<SearchResultDto> Results { get; set; } = new();
    }
}
