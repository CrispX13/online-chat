using OnlineChatBackend.DTOs;
using OnlineChatBackend.Models;

namespace OnlineChatBackend.Interfaces
{
    public interface IDialogsRepository
    {
        public Dialog AddDialog(DialogPostDTO dialog, int currentUserId);
        public Dialog? DeleteDialog(int dialogId, int currentUserId);
        public Dialog? GetDialog(DialogPostDTO dialog, int currentUserId);
    }
}
