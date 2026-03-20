using OnlineChatBackend.DTOs;
using OnlineChatBackend.Models;

namespace OnlineChatBackend.Interfaces
{
    public interface IChatsRepository
    {
        public Chat AddDirectChat(ChatPostDTO chat, int currentUserId);
        public Chat? DeleteDirectChat(int chatId, int currentUserId);
        public Chat? GetDirectChat(ChatPostDTO chat, int currentUserId);
    }
}
