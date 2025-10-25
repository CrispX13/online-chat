using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineChatBackend.DbContexts;
using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OnlineChatBackend.Repositories
{
    public class ContactsDBRepository : IContactsRepository
    {
        private readonly AppDbContext _context;

        public ContactsDBRepository(AppDbContext context)
        {
            _context = context;
        }

        public Contact AddByName(string Name)
        {
            Contact contact = new Contact();
            contact.Name = Name;
            _context.Contacts.Add(contact);
            _context.SaveChanges();
            return contact;
        }

        public void CreateContact(Contact contact)
        {
            //var createdContact = _context.Contacts.FirstOrDefault(x => x.Name == Name);
            //if (createdContact != null) {
            _context.Contacts.Add(contact);
            _context.SaveChanges();
        }

        public IEnumerable<Contact> GetAll()
        {
            return _context.Contacts.ToList();
        }

        public Contact? GetContact(int Id)
        {
            return _context.Contacts.Find(Id);
        }

        public Contact? RemoveContact(int Id)
        {
            var contact = _context.Contacts.Find(Id);
            if (contact == null)
            {
                return null;
            }
            else
            {
                _context.Contacts.Remove(contact);
                _context.SaveChanges() ;
                return contact;
            }
        }

        public Contact? UpdateContact(Contact contact)
        {
            //Дописать, не работает обновление
            var foundContact = _context.Contacts.Find(contact.Id);
            if (foundContact != null)
            { 
                foundContact.Name = contact.Name;
                _context.SaveChanges();
            }

            return foundContact;
        }
        
        public Contact? FindByName(string Name)
        {
            return _context.Contacts.FirstOrDefault(x => x.Name == Name);
        }

        public IEnumerable<Contact> FindAllForId(int id)
        {
            var contacts =  _context.Dialogs
                .Where(d => d.FirstUserId == id || d.SecondUserId == id)
                .Select(d => d.FirstUserId == id ? d.SecondUser : d.FirstUser)
                .Distinct()                
                .AsNoTracking()
                .ToList();
            return contacts;
        }

        public IEnumerable<Contact>? Search(string PartOfName)
        {
            return _context.Contacts.Where(x => EF.Functions.ILike(x.Name, $"%{PartOfName}%"));
        }
    }
}
