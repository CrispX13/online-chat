using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineChatBackend.DTOs;
using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Models;

namespace OnlineChatBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DialogController : Controller
    {
        private IDialogsRepository _dialogsRepository;

        public DialogController(IDialogsRepository dialogsRepository)
        {
            _dialogsRepository = dialogsRepository;
        }

        [HttpPost]
        public IActionResult NewDialog([FromBody] DialogPostDTO dialog)
        {
            int currentUserId = int.Parse(User.FindFirst("id")!.Value);
            var Key = _dialogsRepository.AddDialog(dialog, currentUserId);

            if (Key == null)
                return NotFound();

            return Ok(Key);
        }

        [HttpGet]
        public IActionResult GetDialog([FromQuery]DialogPostDTO dialog)
        {
            dialog.Normalize();
            int currentUserId = int.Parse(User.FindFirst("id")!.Value);
            var dialogKey = _dialogsRepository.GetDialog(dialog, currentUserId);

            if (dialogKey == null)
            {
                return BadRequest();
            }
            else
                return Ok(new { dialogKey });
        }
    }
}
