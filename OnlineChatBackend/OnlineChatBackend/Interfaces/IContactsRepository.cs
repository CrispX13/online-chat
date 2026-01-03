using OnlineChatBackend.DTOs;
using OnlineChatBackend.Models;

namespace OnlineChatBackend.Interfaces
{
    public interface IContactsRepository
    {
        //void Add(Contact contact);
        IEnumerable<Contact> GetAll();
        Contact? GetContact(int Id);
        Contact? RemoveContact(int Id);
        Contact? UpdateContact(Contact contact);

        Contact AddByName(string Name);

        IEnumerable<ContactWithStatusDto> FindAllForId(int Id);

        IEnumerable<Contact>? Search(string PartOfName);
    }
}
