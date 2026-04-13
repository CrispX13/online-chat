using OnlineChatBackend.Models;

namespace OnlineChatBackend.Interfaces
{
    public interface ISearchSessionStore
    {
        SearchSession? Get(int userId, int chatId);
        void Set(int userId, int chatId, SearchSession session);
        void Clear(int userId, int chatId);
        void ClearAllForUser(int userId);
    }
}
