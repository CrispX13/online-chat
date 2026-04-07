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
        if (dto.UserKey1 != currentUserId && dto.UserKey2 != currentUserId)
            throw new UnauthorizedAccessException("Нельзя создавать диалоги для других пользователей.");

        if (dto.UserKey1 == dto.UserKey2)
            throw new ArgumentException("Нельзя создать диалог с самим собой.");

        var existing = _context.Chats.FirstOrDefault(x =>
            x.Type == ChatType.Direct &&
            (
                (x.FirstUserId == dto.UserKey1 && x.SecondUserId == dto.UserKey2) ||
                (x.FirstUserId == dto.UserKey2 && x.SecondUserId == dto.UserKey1)
            ));

        if (existing != null)
            return existing;

        var newChat = new Chat(dto.UserKey1, dto.UserKey2)
        {
            Type = ChatType.Direct
        };

        _context.Chats.Add(newChat);

        _context.ChatParticipants.AddRange(
            new ChatParticipant
            {
                Chat = newChat,
                UserId = dto.UserKey1,
                IsAdmin = false,
                JoinedAt = DateTime.UtcNow
            },
            new ChatParticipant
            {
                Chat = newChat,
                UserId = dto.UserKey2,
                IsAdmin = false,
                JoinedAt = DateTime.UtcNow
            }
        );

        _context.SaveChanges();
        return newChat;
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

    public Chat CreateGroupChat(string name, int ownerUserId, IEnumerable<int> participantIds)
    {
        // имя можно валидировать, обрезать и т.п.
        var chat = new Chat
        {
            Type = ChatType.Group,
            Name = name,
            AvatarUrl = null // можно задать дефолт
        };

        _context.Chats.Add(chat);
        _context.SaveChanges(); // нужен Id для Participants

        var allIds = participantIds
            .Where(id => id != ownerUserId)
            .Distinct()
            .ToList();
        allIds.Add(ownerUserId); // владелец тоже участник

        var now = DateTimeOffset.UtcNow;

        foreach (var userId in allIds)
        {
            _context.ChatParticipants.Add(new ChatParticipant
            {
                ChatId = chat.Id,
                UserId = userId,
                IsAdmin = userId == ownerUserId,
                JoinedAt = now
            });
        }

        _context.SaveChanges();
        return chat;
    }

    public Chat? GetChatById(int chatId, int currentUserId)
    {
        return _context.Chats
            .Include(c => c.Participants)
            .FirstOrDefault(c =>
                c.Id == chatId &&
                (
                    (c.Type == ChatType.Direct &&
                     (c.FirstUserId == currentUserId || c.SecondUserId == currentUserId))
                    ||
                    (c.Type == ChatType.Group &&
                     c.Participants.Any(p => p.UserId == currentUserId))
                ));
    }
}