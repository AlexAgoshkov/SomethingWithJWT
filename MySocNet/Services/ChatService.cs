using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MimeKit.Encodings;
using MySocNet.Models;
using MySocNet.Response;
using MySocNet.Services.Interfaces;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MySocNet.Services
{
    public class ChatService : IChatService
    {
        private readonly IRepository<Chat> _chatRepository;
        private readonly IRepository<UserChat> _userChatRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Message> _messageRepository;
        private readonly IMapper _mapper;

        public ChatService(
            IRepository<User> userRepository, 
            IRepository<UserChat> userChatRepository, 
            IRepository<Chat> chatRepository, 
            IRepository<Message> messageRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _userChatRepository = userChatRepository;
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<Chat> AddNewUserToChatAsync(int chatId, int userId)
        {
            var chat = await _chatRepository.GetWhere(x => x.Id == chatId)
                .Include(x => x.UserChats).FirstOrDefaultAsync();
            chat.UserChats.Add(new UserChat { ChatId = chatId, UserId = userId });
            await _chatRepository.UpdateAsync(chat);
            return chat;
        }

        public async Task<Chat> RemoveUserFromChatAsync(int chatId, int userId)
        {
            var userChat = await _userChatRepository
                .GetWhere(x => x.ChatId == chatId && x.UserId == userId)
                .FirstOrDefaultAsync();
            await _userChatRepository.RemoveAsync(userChat);
            return await _chatRepository.GetByIdAsync(chatId);
        }

        public async Task RemoveChatAsync(int ownerId, int chatId)
        {
            var chat = await _chatRepository.GetWhere(x => x.Id == chatId)
                .Include(x => x.UserChats).Include(x => x.Messages).FirstOrDefaultAsync();
            if (chat.ChatOwnerId == ownerId)
            {
               await _chatRepository.RemoveAsync(chat);
            }
        }

        public async Task<Chat> EditChatAsync(int chatId, string chatName)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);
            if (!string.IsNullOrWhiteSpace(chatName))
            {
                chat.ChatName = chatName;
                await _chatRepository.UpdateAsync(chat);
            }
            return chat;
        }

        public async Task<IList<ChatResponse>> GetChatsAsync(int userId, int skip, int take)
        {          
            var user = await _userRepository.GetWhere(x => x.Id == userId)
                .Include(x => x.UserChats)
                .Skip(skip).Take(take).FirstOrDefaultAsync();

            var chats = new List<ChatResponse>();
           
            foreach (var item in user.UserChats)
            {
                var chat = await _chatRepository.GetWhere(x => x.Id == item.ChatId)
                    .Include(x => x.UserChats).Include(x => x.Messages).FirstOrDefaultAsync();

                if (chat != null && chat.Messages.Count > 0)
                {
                    var msg = chat.Messages.LastOrDefault(); //last message
                    
                    var sender = await _userRepository.GetByIdAsync(msg.SenderId.Value);//last user aka sender
                   
                    chats.Add(new ChatResponse
                    {
                        Id = chat.Id,
                        ChatName = chat.ChatName,
                        UnReadMessageCount = chat.Messages.Count,
                        LastMessage = msg.Text,
                        SenderName = sender.FirstName
                    });
                }
            }

            return chats;
        }

        public async Task<ChatDetailsResponse> GetChatDetailsAsync(int chatId)
        {
            List<string> list = new List<string>(); //todo user list
            var result = new ChatDetailsResponse();
            
            var chat = await _chatRepository.GetWhere(x => x.Id == chatId)
                .Include(x => x.UserChats).FirstOrDefaultAsync();
            result.ChatName = chat.ChatName;

            foreach (var item in chat.UserChats)
            {
                var user = await _userRepository.GetWhere(x => x.Id == item.UserId).FirstOrDefaultAsync();
                list.Add(user.FirstName);
            }
            result.Users = list;

            return result;
        }

        public async Task<IList<Message>> GetChatHistoryAsync(int chatId, int skip, int take)
        {
            return await _messageRepository
                .GetWhere(x => x.ChatId == chatId)
                .Skip(skip).Take(take).ToListAsync();
        }

        public async Task<IList<Message>> GetNewMessagesAsync(int chatId, int skip, int take)
        {
            return await _messageRepository
                .GetWhere(x => x.ChatId == chatId && x.IsRead == false)
                .Skip(skip).Take(take).ToListAsync();
        }

        public async Task GetReadMessageAsync(int userId, int chatId)
        {
            var user = await _userRepository.GetWhere(x => x.Id == userId)
                .Include(x => x.Chats)
                .ThenInclude(x => x.Messages).FirstOrDefaultAsync();
            var chats = user.Chats.Where(x => x.Id == chatId).FirstOrDefault();
            var messages = chats.Messages.ToList();
            foreach (var item in messages)
            {
                if (!item.IsRead)
                {
                    item.IsRead = true;
                }
            }
            await _chatRepository.UpdateAsync(chats);
        }

        public async Task<Chat> CreateChatAsync(string chatName, User owner, int[] usersIds)
        {
            var chat = new Chat { ChatName = chatName, ChatOwner = owner  };

            var users = await GetUsersByIdListAsync(usersIds.Distinct());
            await _userChatRepository.AddAsync(new UserChat { Chat = chat, User = owner });

            foreach (var item in users)
            {
                if (item.Id == owner.Id)
                    continue;

                await _userChatRepository.AddAsync(new UserChat
                {
                    Chat = chat,
                    User = item
                });
            }

            return chat;
        }

        public async Task<Message> SendMessageAsync(int chatId, User sender, string message)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);
            var responseMessage = new Message 
            { 
              ChatId = chatId, 
              Text = message, 
              Sender = sender,
              IsRead = false,
              Time = DateTime.Now
            };
            chat.Messages.Add(responseMessage);

            await _chatRepository.UpdateAsync(chat);

            return responseMessage;
        }

        private async Task<IList<User>> GetUsersByIdListAsync(IEnumerable<int> ids)
        {
            List<User> result = new List<User>();

            foreach (var item in ids)
            {
                var user = await _userRepository.GetByIdAsync(item);

                if (user != null)
                    result.Add(user);
            }

            return result;
        }
    }
}
