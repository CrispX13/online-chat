using OnlineChatBackend.DTOs;
using OnlineChatBackend.Models;

namespace OnlineChatBackend.Interfaces
{
    public interface IChatsRepository
    {
        public Chat AddDirectChat(ChatPostDTO chat, int currentUserId);
        public Chat? DeleteDirectChat(int chatId, int currentUserId);
        Chat CreateGroupChat(string name, int ownerUserId, IEnumerable<int> participantIds);
        Chat? GetChatById(int chatId, int currentUserId);
    }
}
