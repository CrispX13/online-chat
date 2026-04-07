using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineChatBackend.Models
{
    public class SearchQuery
    {
        [Key]
        public int Id { get; set; }

        // Кто инициировал поиск
        public int UserId { get; set; }

        // В каком чате выполнялся поиск
        public int ChatId { get; set; }

        // Текст поискового запроса
        [Required]
        [MaxLength(500)]
        public string QueryText { get; set; } = string.Empty;

        // Внешний провайдер/тип поиска (если понадобится различать)
        [MaxLength(100)]
        public string? Provider { get; set; }  // например, "Google", "Bing", "Custom"

        public DateTimeOffset CreatedAt { get; set; }

        public Contact? User { get; set; }
        public Chat? Chat { get; set; }

        // Сообщения‑результаты, относящиеся к этому запросу
        public List<Message> Messages { get; set; } = new();
    }
}