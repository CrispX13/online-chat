using Microsoft.EntityFrameworkCore;
using OnlineChatBackend.DbContexts;
using OnlineChatBackend.DTOs;
using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Models;

public class ChatDBRepository : IChatsRepository
{
    private readonly AppDbContext _context;

    public ChatDBRepository(AppDbContext context)
    {
        _context = context;
    }

    // Создание ЛИЧНОГО чата (аналог AddDialog)
    public Chat AddDirectChat(ChatPostDTO dto, int currentUserId)
    {
        // currentUserId один из участников
        if (dto.UserKey1 != currentUserId && dto.UserKey2 != currentUserId)
            throw new UnauthorizedAccessException("Нельзя создавать диалоги для других пользователей.");

        // Не даём создать сам с собой
        if (dto.UserKey1 == dto.UserKey2)
            throw new ArgumentException("Нельзя создать диалог с самим собой.");

        // Проверяем, что такой Direct-чат пары уже есть (в любую сторону)
        var existing = _context.Chats.FirstOrDefault(x =>
            x.Type == ChatType.Direct &&
            (
                (x.FirstUserId == dto.UserKey1 && x.SecondUserId == dto.UserKey2) ||
                (x.FirstUserId == dto.UserKey2 && x.SecondUserId == dto.UserKey1)
            ));

        if (existing != null)
            return existing;

        // Конструктор, который нормализует порядок пользователей
        var newChat = new Chat(dto.UserKey1, dto.UserKey2)  // такой же, как был в Dialog
        {
            Type = ChatType.Direct
        };

        _context.Chats.Add(newChat);
        _context.SaveChanges();
        return newChat;
    }

    // Получить чат по Id (любой: Direct или Group), только если участвует currentUserId
    public Chat? GetChatById(int chatId, int currentUserId)
    {
        return _context.Chats
            .Include(c => c.Participants)
            .FirstOrDefault(c =>
                c.Id == chatId &&
                (
                    // Direct-чат — через First/SecondUserId
                    (c.Type == ChatType.Direct &&
                        (c.FirstUserId == currentUserId || c.SecondUserId == currentUserId))
                    ||
                    // Group-чат — через участников
                    (c.Type == ChatType.Group &&
                        c.Participants.Any(p => p.UserId == currentUserId))
                ));
    }

    // Удалить ЛИЧНЫЙ чат (как DeleteDialog)
    public Chat? DeleteDirectChat(int chatId, int currentUserId)
    {
        var chat = _context.Chats.FirstOrDefault(c =>
            c.Id == chatId &&
            c.Type == ChatType.Direct &&
            (c.FirstUserId == currentUserId || c.SecondUserId == currentUserId));

        if (chat == null)
            return null;

        _context.Chats.Remove(chat);
        _context.SaveChanges();
        return chat;
    }

    // Найти ЛИЧНЫЙ чат по паре пользователей (аналог GetDialog)
    public Chat? GetDirectChat(ChatPostDTO dto, int currentUserId)
    {
        // currentUserId должен быть одним из участников
        if (currentUserId != dto.UserKey1 && currentUserId != dto.UserKey2)
            return null;

        return _context.Chats.FirstOrDefault(c =>
            c.Type == ChatType.Direct &&
            (
                (c.FirstUserId == dto.UserKey1 && c.SecondUserId == dto.UserKey2) ||
                (c.FirstUserId == dto.UserKey2 && c.SecondUserId == dto.UserKey1)
            ));
    }

    // Пример: создать ГРУППОВОЙ чат
    public Chat AddGroupChat(string name, int ownerUserId, IEnumerable<int> participantIds)
    {
        var chat = new Chat
        {
            Type = ChatType.Group,
            Name = name
        };

        _context.Chats.Add(chat);
        _context.SaveChanges(); // нужно Id для ChatParticipant

        // добавляем участников
        var allIds = participantIds.Distinct().ToList();
        if (!allIds.Contains(ownerUserId))
            allIds.Add(ownerUserId);

        foreach (var userId in allIds)
        {
            _context.ChatParticipants.Add(new ChatParticipant
            {
                ChatId = chat.Id,
                UserId = userId,
                IsAdmin = userId == ownerUserId,
                JoinedAt = DateTimeOffset.UtcNow
            });
        }

        _context.SaveChanges();
        return chat;
    }
}