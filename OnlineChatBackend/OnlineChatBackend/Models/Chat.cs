using System.ComponentModel.DataAnnotations;

namespace OnlineChatBackend.Models
{
    public enum ChatType
    {
        Direct = 0,
        Group = 1
    }

    public class Chat
    {
        [Key]
        public int Id { get; set; }

        public ChatType Type { get; set; }

        // Для группового
        public string? Name { get; set; }
        public string? AvatarUrl { get; set; }

        // Для личного (оставляешь для обратной совместимости)
        public int? FirstUserId { get; set; }
        public int? SecondUserId { get; set; }

        public Chat() { }

        public Chat(int FirstUserId, int SecondUserId)
        {
            //нормализую ID для ускорения поиска

            this.FirstUserId = FirstUserId > SecondUserId ? SecondUserId : FirstUserId;
            this.SecondUserId = FirstUserId < SecondUserId ? SecondUserId : FirstUserId;

        }

        public List<Message> Messages { get; set; } = new();

        // Many-to-many участники
        public List<ChatParticipant> Participants { get; set; } = new();

        public List<Notification> Notifications { get; set; } = new();
    }
}
