using OnlineChatBackend.DbContexts;
using OnlineChatBackend.DTOs;
using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Models;

namespace OnlineChatBackend.Repositories
{
    public class DialogDBRepository : IDialogsRepository
    {
        private AppDbContext _context;

        public DialogDBRepository(AppDbContext context)
        {
            _context = context;
        }
        public Dialog AddDialog(DialogPostDTO dialog)
        {
            Dialog newDialog = new Dialog(dialog.UserKey1, dialog.UserKey2);
            _context.Dialogs.Add(newDialog);
            _context.SaveChanges();
            return newDialog;
        }

        public Dialog? DeleteDialog(int Id)
        {
            var foundDialog = _context.Dialogs.Find(Id);
            if (foundDialog != null)
            {
                _context.Dialogs.Remove(foundDialog);
                return foundDialog;
            }
            return null;
        }

        public Dialog? GetDialog(DialogPostDTO dialog)
        {
            return _context.Dialogs.FirstOrDefault(x => x.FirstUserId == dialog.UserKey1 &&  x.SecondUserId == dialog.UserKey2);
        }
    }
}
