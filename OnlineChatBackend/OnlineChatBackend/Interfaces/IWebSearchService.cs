using OnlineChatBackend.DTOs;

namespace OnlineChatBackend.Interfaces
{
    public interface IWebSearchService
    {
        Task<IReadOnlyList<SearchResultDto>> SearchAsync(string query, CancellationToken ct = default);
    }
}
