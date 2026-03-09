using OnlineChatBackend.Models;

namespace OnlineChatBackend.Interfaces
{
    public interface IMessageRepository
    {
        void Add(Message message);
        Message? Update(int messageId, string newText, int currentUserId);
        Message? Delete(int messageId, int currentUserId);
        List<Message> GetAll(int dialogId, int currentUserId);
        Message? GetLastMessage(int dialogId, int currentUserId);
    }

}
