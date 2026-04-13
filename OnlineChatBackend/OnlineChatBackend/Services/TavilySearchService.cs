using OnlineChatBackend.DTOs;
using OnlineChatBackend.Interfaces;
using System.Net.Http;
using System.Text;
using System.Text.Json;
namespace OnlineChatBackend.Services
{
    public class TavilySearchService : IWebSearchService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public TavilySearchService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<IReadOnlyList<SearchResultDto>> SearchAsync(
            string query,
            CancellationToken ct = default)
        {
            var apiKey = _configuration["Search:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Search:ApiKey is not configured.");

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.tavily.com/search");
            request.Headers.Add("Authorization", $"Bearer {apiKey}");

            var body = new
            {
                query = query,
                search_depth = "basic",  // или "advanced", если захочешь глубже [web:299][web:330]
                max_results = 5,
                include_answer = false,
                include_raw_content = false,
                topic = "general"
            };

            request.Content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json"
            );

            using var response = await _httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            var list = new List<SearchResultDto>();

            if (doc.RootElement.TryGetProperty("results", out var results))
            {
                foreach (var item in results.EnumerateArray())
                {
                    var title = item.TryGetProperty("title", out var t) ? t.GetString() ?? "" : "";
                    var url = item.TryGetProperty("url", out var u) ? u.GetString() ?? "" : "";
                    var text = item.TryGetProperty("content", out var c) ? c.GetString() ?? "" : "";

                    list.Add(new SearchResultDto
                    {
                        Title = title,
                        Url = url,
                        Snippet = text
                    });
                }
            }

            return list;
        }
    }
}
