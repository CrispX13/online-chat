using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineChatBackend.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public int? ToUserId { get; set; }
        public int? FromUserId { get; set; }

        public int ChatId { get; set; }

        [Required]
        public required string MessageText { get; set; }

        public DateTimeOffset MessageDateTime { get; set; }

        public bool Changed { get; set; } = false;

        // Пометить, что это результат поиска (опционально)
        public bool IsSearchResult { get; set; } = false;

        // Связь с поисковым запросом (если сообщение – результат или сам запрос)
        public int? SearchQueryId { get; set; }

        public Chat? Chat { get; set; }
        public SearchQuery? SearchQuery { get; set; }

    }
}
