using OnlineChatBackend.DbContexts;
using OnlineChatBackend.Models;

namespace OnlineChatBackend.Repositories
{
    public class AccountRepository(ContactsDBRepository contactsDB)
    {
        public void Add(Contact account)
        {
            contactsDB.CreateContact(account);
        }

        public Contact? GetByUserName(string UserName)
        {
            return contactsDB.FindByName(UserName);
        }
    }
}
