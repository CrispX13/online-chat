using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineChatBackend.DbContexts;
using OnlineChatBackend.DTOs;
using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Models;
using OnlineChatBackend.Repositories;
using SignalRSwaggerGen.Attributes;
using System.Collections.Concurrent;
using System.Text;

namespace OnlineChatBackend.Hubs
{
    [Authorize]
    [SignalRHub]
    public class ChatHub:Hub
    {


        private readonly IWebSearchService _webSearchService;
        private readonly ISearchSessionStore _searchSessionStore;
        public IMessageRepository messageRepository { get; set; }

        public IChatsRepository chatsRepository { get; set; }

        private readonly AppDbContext _db;


        private readonly ILogger<ChatHub> _log;

        public ChatHub(
            IMessageRepository messageRepository, 
            ILogger<ChatHub> log,
            IChatsRepository ChatsRepository,
            AppDbContext db,
            IWebSearchService webSearchService,
            ISearchSessionStore searchSessionStore)
        {
            this.messageRepository = messageRepository;
            this.chatsRepository = ChatsRepository;
            _log = log;
            _db = db;
            _webSearchService = webSearchService;
            _searchSessionStore = searchSessionStore;
        }

        public async Task SendMessage(string message, string chatId)
        {
            _log.LogInformation("SendMessage: raw chatId = {ChatId}, userId = {UserId}", chatId, Context.UserIdentifier);

            foreach (var claim in Context.User?.Claims ?? Enumerable.Empty<System.Security.Claims.Claim>())
            {
                _log.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
            }

            if (!int.TryParse(Context.UserIdentifier, out var fromUserId))
                throw new HubException("Пользователь не авторизован.");

            if (!int.TryParse(chatId, out var chatIdInt))
                throw new HubException("Некорректный chatId.");

            if (string.IsNullOrWhiteSpace(message))
                throw new HubException("Пустое сообщение отправить нельзя.");

            message = message.Trim();

            var chat = await _db.Chats
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == chatIdInt);

            if (chat == null)
                throw new HubException("Чат не найден.");

            var participantIds = chat.Participants
                .Select(p => p.UserId)
                .Distinct()
                .ToList();

            if (!participantIds.Contains(fromUserId))
                throw new HubException("Вы не являетесь участником этого чата.");

            if (string.Equals(message, "/stop", StringComparison.OrdinalIgnoreCase))
            {
                _searchSessionStore.Clear(fromUserId, chatIdInt);

                await Clients.Caller.SendAsync("SearchCleared", new SearchClearedDTO
                {
                    ChatId = chatIdInt
                });

                return;
            }

            if (message.StartsWith("/googling", StringComparison.OrdinalIgnoreCase))
            {
                var query = message["/googling".Length..].Trim();

                if (string.IsNullOrWhiteSpace(query))
                    throw new HubException("Использование: /googling ваш запрос");

                try
                {
                    var results = await _webSearchService.SearchAsync(query);

                    var session = new SearchSession
                    {
                        Query = query,
                        Results = results.ToList()
                    };

                    _searchSessionStore.Set(fromUserId, chatIdInt, session);

                    var dto = new SearchResultsDTO
                    {
                        ChatId = chatIdInt,
                        Query = query,
                        Results = session.Results.Select((x, index) => new SearchResultItemDTO
                        {
                            Index = index,
                            Title = x.Title,
                            Url = x.Url,
                            Snippet = TrimText(x.Snippet)
                        }).ToList()
                    };

                    await Clients.Caller.SendAsync("SearchResultsReceived", dto);
                    return;
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Ошибка при /googling для chatId={ChatId}, userId={UserId}", chatIdInt, fromUserId);

                    await Clients.Caller.SendAsync("SearchError", new SearchErrorDTO
                    {
                        ChatId = chatIdInt,
                        Message = "Ошибка при выполнении веб-поиска."
                    });

                    return;
                }
            }

            await SaveAndBroadcastMessage(chat, participantIds, fromUserId, message);
        }

        public async Task SendSearchSelection(SendSearchSelectionDTO request)
        {
            _log.LogInformation("SendSearchSelection: raw chatId = {ChatId}, userId = {UserId}", request.ChatId, Context.UserIdentifier);

            if (request == null)
                throw new HubException("Пустой запрос.");

            if (!int.TryParse(Context.UserIdentifier, out var fromUserId))
                throw new HubException("Пользователь не авторизован.");

            if (!int.TryParse(request.ChatId, out var chatIdInt))
                throw new HubException("Некорректный chatId.");

            if (request.SelectedIndexes == null || request.SelectedIndexes.Count == 0)
                throw new HubException("Не выбраны результаты для отправки.");

            var chat = await _db.Chats
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == chatIdInt);

            if (chat == null)
                throw new HubException("Чат не найден.");

            var participantIds = chat.Participants
                .Select(p => p.UserId)
                .Distinct()
                .ToList();

            if (!participantIds.Contains(fromUserId))
                throw new HubException("Вы не являетесь участником этого чата.");

            var session = _searchSessionStore.Get(fromUserId, chatIdInt);
            if (session == null)
                throw new HubException("Нет активного поиска для этого чата.");

            var normalizedIndexes = request.SelectedIndexes
                .Distinct()
                .Where(i => i >= 0 && i < session.Results.Count)
                .OrderBy(i => i)
                .ToList();

            if (normalizedIndexes.Count == 0)
                throw new HubException("Выбранные результаты некорректны.");

            var selectedResults = normalizedIndexes
                .Select(i => session.Results[i])
                .ToList();

            var text = BuildSelectedSearchResultsMessage(session.Query, selectedResults);

            await SaveAndBroadcastMessage(chat, participantIds, fromUserId, text);

            _searchSessionStore.Clear(fromUserId, chatIdInt);

            await Clients.Caller.SendAsync("SearchCleared", new SearchClearedDTO
            {
                ChatId = chatIdInt
            });
        }

        private async Task SaveAndBroadcastMessage(
            Chat chat,
            List<int> participantIds,
            int fromUserId,
            string messageText)
        {
            int? toUserId = null;

            if (chat.Type == ChatType.Direct)
            {
                toUserId = participantIds.FirstOrDefault(id => id != fromUserId);

                if (toUserId == 0)
                    throw new HubException("Второй участник Direct-чата не найден.");
            }

            var newMessage = new Message
            {
                MessageText = messageText.Trim(),
                ChatId = chat.Id,
                ToUserId = toUserId,
                FromUserId = fromUserId,
                MessageDateTime = DateTimeOffset.UtcNow
            };

            messageRepository.Add(newMessage);

            var dto = new MessageHubDTO
            {
                Id = newMessage.Id,
                ChatId = newMessage.ChatId,
                FromUserId = newMessage.FromUserId,
                ToUserId = newMessage.ToUserId,
                MessageText = newMessage.MessageText,
                MessageDateTime = newMessage.MessageDateTime,
                Changed = newMessage.Changed
            };

            foreach (var uid in participantIds)
            {
                await Clients.User(uid.ToString()).SendAsync("MessageCreated", dto);
            }
        }

        private static string BuildSelectedSearchResultsMessage(
            string query,
            IReadOnlyList<SearchResultDto> results)
        {
            if (results == null || results.Count == 0)
                return $"По запросу \"{query}\" ничего не выбрано.";

            var sb = new StringBuilder();
            sb.AppendLine($"Результаты поиска по запросу: {query}");
            sb.AppendLine();

            for (int i = 0; i < results.Count; i++)
            {
                var item = results[i];
                sb.AppendLine($"{i + 1}. {item.Title}");
                sb.AppendLine(item.Url);

                var snippet = TrimText(item.Snippet);
                if (!string.IsNullOrWhiteSpace(snippet))
                    sb.AppendLine(snippet);

                sb.AppendLine();
            }

            return sb.ToString().Trim();
        }

        private async Task HandleGooglingCommand(
            string message,
            Chat chat,
            int chatIdInt,
            int fromUserId,
            List<int> participantIds)
        {
            var query = message["/googling".Length..].Trim();

            if (string.IsNullOrWhiteSpace(query))
                throw new HubException("Использование: /googling ваш запрос");

            int? toUserId = null;
            if (chat.Type == ChatType.Direct)
            {
                toUserId = participantIds.FirstOrDefault(id => id != fromUserId);
                if (toUserId == 0)
                    throw new HubException("Второй участник Direct-чата не найден.");
            }

            var userCommandMessage = new Message
            {
                MessageText = message.Trim(),
                ChatId = chatIdInt,
                ToUserId = toUserId,
                FromUserId = fromUserId,
                MessageDateTime = DateTimeOffset.UtcNow
            };

            messageRepository.Add(userCommandMessage);

            var userDto = new MessageHubDTO
            {
                Id = userCommandMessage.Id,
                ChatId = userCommandMessage.ChatId,
                FromUserId = userCommandMessage.FromUserId,
                ToUserId = userCommandMessage.ToUserId,
                MessageText = userCommandMessage.MessageText,
                MessageDateTime = userCommandMessage.MessageDateTime,
                Changed = userCommandMessage.Changed
            };

            foreach (var uid in participantIds)
            {
                await Clients.User(uid.ToString()).SendAsync("MessageCreated", userDto);
            }

            try
            {
                var results = await _webSearchService.SearchAsync(query);
                var botText = FormatSearchResults(results, query);

                var botMessage = new Message
                {
                    MessageText = botText,
                    ChatId = chatIdInt,
                    ToUserId = toUserId,
                    FromUserId = fromUserId, // лучше потом заменить на BotUserId
                    MessageDateTime = DateTimeOffset.UtcNow
                };

                messageRepository.Add(botMessage);

                var botDto = new MessageHubDTO
                {
                    Id = botMessage.Id,
                    ChatId = botMessage.ChatId,
                    FromUserId = botMessage.FromUserId,
                    ToUserId = botMessage.ToUserId,
                    MessageText = botMessage.MessageText,
                    MessageDateTime = botMessage.MessageDateTime,
                    Changed = botMessage.Changed
                };

                foreach (var uid in participantIds)
                {
                    await Clients.User(uid.ToString()).SendAsync("MessageCreated", botDto);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Ошибка при /googling для chatId={ChatId}, userId={UserId}", chatIdInt, fromUserId);

                var errorMessage = new Message
                {
                    MessageText = "Ошибка при выполнении веб-поиска.",
                    ChatId = chatIdInt,
                    ToUserId = toUserId,
                    FromUserId = fromUserId, // лучше потом заменить на BotUserId
                    MessageDateTime = DateTimeOffset.UtcNow
                };

                messageRepository.Add(errorMessage);

                var errorDto = new MessageHubDTO
                {
                    Id = errorMessage.Id,
                    ChatId = errorMessage.ChatId,
                    FromUserId = errorMessage.FromUserId,
                    ToUserId = errorMessage.ToUserId,
                    MessageText = errorMessage.MessageText,
                    MessageDateTime = errorMessage.MessageDateTime,
                    Changed = errorMessage.Changed
                };

                foreach (var uid in participantIds)
                {
                    await Clients.User(uid.ToString()).SendAsync("MessageCreated", errorDto);
                }
            }
        }

        private static string FormatSearchResults(IReadOnlyList<SearchResultDto> results, string query)
        {
            if (results == null || results.Count == 0)
                return $"По запросу \"{query}\" ничего не найдено.";

            var sb = new StringBuilder();
            sb.AppendLine($"Результаты поиска по запросу: {query}");
            sb.AppendLine();

            for (int i = 0; i < Math.Min(results.Count, 3); i++)
            {
                var item = results[i];
                sb.AppendLine($"{i + 1}. {item.Title}");
                sb.AppendLine(item.Url);
                sb.AppendLine(TrimText(item.Snippet));
                sb.AppendLine();
            }

            return sb.ToString().Trim();
        }

        private static string TrimText(string? text, int maxLength = 220)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            text = text.Replace("\r", " ").Replace("\n", " ").Trim();

            return text.Length <= maxLength
                ? text
                : text[..maxLength] + "...";
        }

        public async Task NewContact(int userId, int newContactId)
        {
            var callerUserId = int.Parse(Context.UserIdentifier!);

            // создаём/находим direct-чат
            var chat = chatsRepository.AddDirectChat(new ChatPostDTO(userId, newContactId), callerUserId);

            // для обоих пользователей ставим NewContact = true
            await MarkNewContactAsync(chat.Id, userId);
            await MarkNewContactAsync(chat.Id, newContactId);

            // просигналить фронту, чтобы тот вызвал refreshContacts()
            await Clients.Caller.SendAsync("NewChat", true);
            await Clients.User(newContactId.ToString()).SendAsync("NewChat", true);
        }

        private async Task MarkNewContactAsync(int chatId, int userId)
        {
            var notif = _db.Notifications
                .FirstOrDefault(n => n.ChatId == chatId && n.UserId == userId);

            if (notif == null)
            {
                notif = new Notification
                {
                    ChatId = chatId,
                    UserId = userId,
                    NewNotifications = false,
                    NewContact = true
                };
                _db.Notifications.Add(notif);
            }
            else
            {
                notif.NewContact = true;
            }

            await _db.SaveChangesAsync();
        }

        public async Task EditMessage(int messageId, string newText)
        {
            int currentUserId = int.Parse(Context.UserIdentifier!);

            var updated = messageRepository.Update(messageId, newText, currentUserId);
            if (updated == null)
                throw new HubException("Сообщение не найдено или нет прав.");

            var dto = new MessageHubDTO
            {
                Id = updated.Id,
                ChatId = updated.ChatId,
                FromUserId = updated.FromUserId,
                ToUserId = updated.ToUserId,
                MessageText = updated.MessageText,
                MessageDateTime = updated.MessageDateTime,
                Changed = updated.Changed
            };

            // Получаем всех участников чата, а не только FromUserId/ToUserId
            var chat = await _db.Chats
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == updated.ChatId);

            if (chat != null)
            {
                foreach (var uid in chat.Participants.Select(p => p.UserId).Distinct())
                    await Clients.User(uid.ToString()).SendAsync("MessageEdited", dto);
            }
        }

        public async Task DeleteMessage(int messageId)
        {
            int currentUserId = int.Parse(Context.UserIdentifier!);

            var deleted = messageRepository.Delete(messageId, currentUserId);
            if (deleted == null)
                throw new HubException("Сообщение не найдено или нет прав.");

            var chat = await _db.Chats
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == deleted.ChatId);

            if (chat != null)
            {
                foreach (var uid in chat.Participants.Select(p => p.UserId).Distinct())
                    await Clients.User(uid.ToString()).SendAsync("MessageDeleted", deleted.Id);
            }
        }

    }
}
