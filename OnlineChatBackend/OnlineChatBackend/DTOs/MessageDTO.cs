using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace OnlineChatBackend.DTOs
{
    public class MessageDTO
    {
        [Required]
        public int DialogId { get; set; }
        [Required]
        [NotNull]
        [MinLength(1)]
        public string TextMessage { get; set; } = null!;
    }
}
