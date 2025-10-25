using OnlineChatBackend.DTOs;
using OnlineChatBackend.Models;

namespace OnlineChatBackend.Interfaces
{
    public interface IDialogsRepository
    {
        public Dialog AddDialog(DialogPostDTO dialog);
        public Dialog? DeleteDialog(int Id);
        public Dialog? GetDialog(DialogPostDTO dialog);
    }
}
