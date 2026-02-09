using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineChatBackend.DbContexts;
using OnlineChatBackend.DTOs;
using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace OnlineChatBackend.Repositories
{
    public class ContactsDBRepository : IContactsRepository
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private const string DefaultAvatarRelativePath = "avatars/default.png";
        private const string AvatarsFolder = "avatars";

        public ContactsDBRepository(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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

        public IEnumerable<ContactWithStatusDto> FindAllForId(int id)
        {
            var contacts = _context.Dialogs
                .Where(d => d.FirstUserId == id || d.SecondUserId == id)
                .Select(d => new
                {
                    Dialog = d,
                    OtherUser = d.FirstUserId == id ? d.SecondUser : d.FirstUser,
                    OtherUserId = d.FirstUserId == id ? d.SecondUserId : d.FirstUserId
                })
                .Select(x => new ContactWithStatusDto
                {
                    Contact = x.OtherUser,
                    NewNotifications = _context.Notifications
                        .Any(n => n.DialogId == x.Dialog.Id
                                  && n.UserId == id
                                  && n.NewNotifications),
                    NewContact = _context.Notifications
                        .Any(n => n.DialogId == x.Dialog.Id
                                  && n.UserId == id
                                  && n.NewContact)
                            })
                .AsNoTracking()
                .ToList();

            return contacts;
        }

        public IEnumerable<Contact>? Search(string PartOfName)
        {
            return _context.Contacts.Where(x => EF.Functions.ILike(x.Name, $"%{PartOfName}%"));
        }

        public bool ChangeUserName(int Id, string NewName)
        {
            Contact? contact = _context.Contacts.Find(Id);

            if (contact == null)
            {
                return false;
            }

            contact.Name = NewName;
            _context.SaveChanges();

            return true;
        }

        public bool ChangePassword(int Id, string Password)
        {
            Contact? contact = _context.Contacts.Find(Id);

            if (contact == null)
            {
                return false;
            }

            contact.PasswordHash = Password;
            _context.SaveChanges();

            return true;
        }

        public bool SetDefaultAvatar(int id)
        {
            var contact = _context.Contacts.Find(id);
            if (contact == null)
                return false;

            // если была кастомная аватарка – удаляем файл
            if (!string.IsNullOrEmpty(contact.AvatarUrl) &&
                !string.Equals(contact.AvatarUrl, DefaultAvatarRelativePath, StringComparison.OrdinalIgnoreCase))
            {
                DeleteAvatarFile(contact.AvatarUrl);
            }

            contact.AvatarUrl = DefaultAvatarRelativePath;
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> ChangeAvatarAsync(int id, IFormFile avatarFile)
        {
            var contact = _context.Contacts.Find(id);
            if (contact == null)
                return false;

            if (avatarFile == null || avatarFile.Length == 0)
                return false;

            // при желании: проверка типа файла (ContentType image/*), размера и т.п.

            // папка wwwroot/avatars
            var avatarsRoot = Path.Combine(_env.WebRootPath, AvatarsFolder);
            if (!Directory.Exists(avatarsRoot))
            {
                Directory.CreateDirectory(avatarsRoot);
            }

            // генерируем уникальное имя
            var extension = Path.GetExtension(avatarFile.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(avatarsRoot, fileName);

            // удаляем старый файл, если был кастомный
            if (!string.IsNullOrEmpty(contact.AvatarUrl) &&
                !string.Equals(contact.AvatarUrl, DefaultAvatarRelativePath, StringComparison.OrdinalIgnoreCase))
            {
                DeleteAvatarFile(contact.AvatarUrl);
            }

            // сохраняем новый файл
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await avatarFile.CopyToAsync(stream);
            }

            // сохраняем относительный путь для <img src="/avatars/....">
            contact.AvatarUrl = Path.Combine(AvatarsFolder, fileName).Replace("\\", "/");
            _context.SaveChanges();

            return true;
        }

        public bool DeleteAvatar(int id)
        {
            var contact = _context.Contacts.Find(id);
            if (contact == null)
                return false;

            if (!string.IsNullOrEmpty(contact.AvatarUrl) &&
                !string.Equals(contact.AvatarUrl, DefaultAvatarRelativePath, StringComparison.OrdinalIgnoreCase))
            {
                DeleteAvatarFile(contact.AvatarUrl);
            }

            contact.AvatarUrl = DefaultAvatarRelativePath;
            _context.SaveChanges();
            return true;
        }

        private void DeleteAvatarFile(string relativePath)
        {
            var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/', '\\'));

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        public string? GetAvatarPath(int id)
        {
            var contact = _context.Contacts.Find(id);
            if (contact == null)
                return null;

            // Если AvatarUrl null/пустой – вернём стандартный путь
            return string.IsNullOrEmpty(contact.AvatarUrl)
                ? "avatars/default.png"
                : contact.AvatarUrl;
        }




    }
}
