using OnlineChatBackend.Models;

namespace OnlineChatBackend.Interfaces
{
    public interface IMessageRepository
    {
        void Add(Message message);
        List<Message> GetAll(int DialogId);
        Message? GetLastMessage(int DialogId);
        Message? Update(Message message);
        Message? Delete(Message message);
    }
}
