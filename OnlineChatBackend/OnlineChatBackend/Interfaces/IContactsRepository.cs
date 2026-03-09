using OnlineChatBackend.DTOs;
using OnlineChatBackend.Models;

namespace OnlineChatBackend.Interfaces
{
    public interface IContactsRepository
    {
        Contact? GetCurrentUser(int currentUserId);
        IEnumerable<ContactWithStatusDto> FindAllForUser(int currentUserId);

        bool ChangeUserName(int currentUserId, string newName);
        bool ChangePassword(int currentUserId, string passwordHash);

        string? GetAvatarPath(int currentUserId);
        Task<bool> ChangeAvatarAsync(int currentUserId, IFormFile avatarFile);
        bool SetDefaultAvatar(int currentUserId);

        // Поиск по имени (контактов/юзеров) – если нужно:
        IEnumerable<Contact>? Search(string partOfName);
    }

}
