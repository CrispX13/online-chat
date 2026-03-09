using OnlineChatBackend.DbContexts;
using OnlineChatBackend.DTOs;
using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Models;

public class DialogDBRepository : IDialogsRepository
{
    private readonly AppDbContext _context;

    public DialogDBRepository(AppDbContext context)
    {
        _context = context;
    }

    public Dialog AddDialog(DialogPostDTO dto, int currentUserId)
    {
        // currentUserId один из участников, второй из dto
        if (dto.UserKey1 != currentUserId && dto.UserKey2 != currentUserId)
            throw new UnauthorizedAccessException("Нельзя создавать диалоги для других пользователей.");

        // Не даём создать сам с собой
        if (dto.UserKey1 == dto.UserKey2)
            throw new ArgumentException("Нельзя создать диалог с самим собой.");

        // Проверяем, что такого диалога пары ещё нет (в любую сторону)
        var existing = _context.Dialogs.FirstOrDefault(x =>
            (x.FirstUserId == dto.UserKey1 && x.SecondUserId == dto.UserKey2) ||
            (x.FirstUserId == dto.UserKey2 && x.SecondUserId == dto.UserKey1));

        if (existing != null)
            return existing;

        var newDialog = new Dialog(dto.UserKey1, dto.UserKey2);
        _context.Dialogs.Add(newDialog);
        _context.SaveChanges();
        return newDialog;
    }

    public Dialog? GetDialogById(int dialogId, int currentUserId)
    {
        return _context.Dialogs.FirstOrDefault(x =>
            x.Id == dialogId &&
            (x.FirstUserId == currentUserId || x.SecondUserId == currentUserId));
    }

    public Dialog? DeleteDialog(int dialogId, int currentUserId)
    {
        var dialog = _context.Dialogs.FirstOrDefault(x =>
            x.Id == dialogId &&
            (x.FirstUserId == currentUserId || x.SecondUserId == currentUserId));

        if (dialog == null)
            return null;

        _context.Dialogs.Remove(dialog);
        _context.SaveChanges();
        return dialog;
    }

    public Dialog? GetDialog(DialogPostDTO dialog, int currentUserId)
    {
        // currentUserId должен быть одним из участников
        if (currentUserId != dialog.UserKey1 && currentUserId != dialog.UserKey2)
            return null;

        return _context.Dialogs.FirstOrDefault(x =>
            (x.FirstUserId == dialog.UserKey1 && x.SecondUserId == dialog.UserKey2) ||
            (x.FirstUserId == dialog.UserKey2 && x.SecondUserId == dialog.UserKey1));
    }
}
