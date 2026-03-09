using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineChatBackend.DTOs;
using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Models;
using SignalRSwaggerGen.Attributes;
using System.Collections.Concurrent;

namespace OnlineChatBackend.Hubs
{
    [Authorize]
    [SignalRHub]
    public class ChatHub:Hub
    {
        public IMessageRepository messageRepository { get; set; }

        public IDialogsRepository dialogsRepository { get; set; }


        private readonly ILogger<ChatHub> _log;

        public ChatHub(
            IMessageRepository messageRepository, 
            ILogger<ChatHub> log,
            IDialogsRepository dialogsRepository)
        {
            this.messageRepository = messageRepository;
            this.dialogsRepository = dialogsRepository;
            _log = log;
        }

        public async Task SendMessage(string Message,string DialogId, string UserId)
        {

            var callerUserIdentifier = Context.UserIdentifier;

            Message newMessage = new Message
            {
                MessageText = Message,
                DialogId = Int32.Parse(DialogId),
                ToUserId = Int32.Parse(UserId),
                FromUserId = Int32.Parse(callerUserIdentifier),
                MessageDateTime = DateTime.UtcNow
            };

            await Clients.Caller.SendAsync("MessageCreated", newMessage);
            await Clients.User(UserId).SendAsync("MessageCreated", newMessage);

            messageRepository.Add(newMessage);

        }

        public async Task NewContact(int UserId, int NewContactId)
        {
            var userId = int.Parse(Context.UserIdentifier!);

            Dialog NewDialog = dialogsRepository.AddDialog(new DialogPostDTO(UserId, NewContactId), userId);

            await Clients.Caller.SendAsync("NewDialog", true);
            await Clients.User(NewContactId.ToString()).SendAsync("NewDialog", true);

        }

        public async Task EditMessage(int messageId, string newText)
        {
            var callerUserIdentifier = Context.UserIdentifier!;
            int currentUserId = int.Parse(callerUserIdentifier);

            var updated = messageRepository.Update(messageId, newText, currentUserId);
            if (updated == null)
                throw new HubException("Сообщение не найдено или нет прав на редактирование.");

            await Clients.User(updated.FromUserId.ToString())
                .SendAsync("MessageEdited", updated);

            await Clients.User(updated.ToUserId.ToString())
                .SendAsync("MessageEdited", updated);
        }

        public async Task DeleteMessage(int messageId)
        {
            var callerUserIdentifier = Context.UserIdentifier!;
            int currentUserId = int.Parse(callerUserIdentifier);

            var deleted = messageRepository.Delete(messageId, currentUserId);
            if (deleted == null)
                throw new HubException("Сообщение не найдено или нет прав на удаление.");

            await Clients.User(deleted.FromUserId.ToString())
                .SendAsync("MessageDeleted", deleted.Id);

            await Clients.User(deleted.ToUserId.ToString())
                .SendAsync("MessageDeleted", deleted.Id);
        }


    }
}
