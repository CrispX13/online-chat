using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OnlineChatBackend.DTOs;
using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Models;

namespace OnlineChatBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContactsController : Controller
    {

        public IContactsRepository ContactsRepository { get; set; }

        public ContactsController(IContactsRepository contactsRepository)
        {
            ContactsRepository = contactsRepository;
        }

        [HttpGet]
        public IEnumerable<Contact> GetAlContacts()
        {
            return ContactsRepository.GetAll();
        }


        [HttpPost("{name}", Name = "PostContactByName")]
        public IActionResult PostContactByName(string name)
        {
            var NewContact = ContactsRepository.AddByName(name);

            return CreatedAtRoute("PostContactByName", new { name = NewContact.Name }, NewContact);
        }


        [HttpPut]
        public IActionResult UpdateContact([FromBody] Contact UpdatedContact)
        {

            var contact = ContactsRepository.GetContact(UpdatedContact.Id);
            if (contact == null)
            {
                return NotFound(new { message = "Объект не найден", key = UpdatedContact.Id });
            }
            
            ContactsRepository.UpdateContact(UpdatedContact);

            return Ok(new
            {
                message = "Объект успешно обновлён",
                contact = UpdatedContact
            });
        }

        [HttpDelete("{key}")]
        public IActionResult DeleteContact(int key)
        {
            if (ContactsRepository.GetContact(key) == null)
            {
                return NotFound("Контакт не найден");
            }

            var contact = ContactsRepository.RemoveContact(key);

            return Ok(new
            {
                message = "Контакт успешно удален",
                contact = contact
            });
        }

        [HttpGet("all-for-id/{id}")]
        public IEnumerable<ContactWithStatusDto> GetAllForId(int id)
        {
            return ContactsRepository.FindAllForId(id);
        }

        [HttpPost("search")]
        public IEnumerable<Contact> Search([FromBody] string PartOfName)
        {
            return ContactsRepository.Search(PartOfName);
        }

        [HttpGet("{id}", Name = "get-contact")]
        public ContactDTO? GetContactById(int id)
        {
            Contact contact =  ContactsRepository.GetContact(id);

            if (contact != null)
            {
                return new ContactDTO
                {
                    Id = contact.Id,
                    Name = contact.Name
                };
            }
            return null;
        }
    }
}
