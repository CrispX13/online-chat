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

        public Contact? GetById(int Id)
        {
            return contactsDB.GetContact(Id);
        }

        public bool ChangePassword(int Id, string Password)
        {
            return contactsDB.ChangePassword(Id, Password);
        }

        public bool ChangeName(int Id, string Name)
        {
            return contactsDB.ChangeUserName(Id, Name);
        }
    }
}
