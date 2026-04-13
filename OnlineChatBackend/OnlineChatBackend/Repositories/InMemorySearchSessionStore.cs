using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Models;
using System.Collections.Concurrent;

namespace OnlineChatBackend.Repositories
{
    public class InMemorySearchSessionStore : ISearchSessionStore
    {
        private readonly ConcurrentDictionary<(int userId, int chatId), SearchSession> _sessions
            = new();

        public SearchSession? Get(int userId, int chatId)
            => _sessions.TryGetValue((userId, chatId), out var s) ? s : null;

        public void Set(int userId, int chatId, SearchSession session)
            => _sessions[(userId, chatId)] = session;

        public void Clear(int userId, int chatId)
            => _sessions.TryRemove((userId, chatId), out _);

        public void ClearAllForUser(int userId)
        {
            var keys = _sessions.Keys.Where(k => k.userId == userId).ToList();
            foreach (var key in keys)
                _sessions.TryRemove(key, out _);
        }
    }
}
