using Microsoft.EntityFrameworkCore;
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
        private IRepository<Chat> _chatRepository;
        private IRepository<UserChat> _userChatRepository;
        private IRepository<User> _userRepository;
        private IRepository<Message> _messageRepository;

        public ChatService(
            IRepository<User> userRepository, 
            IRepository<UserChat> userChatRepository, 
            IRepository<Chat> chatRepository, 
            IRepository<Message> messageRepository)
        {
            _userRepository = userRepository;
            _userChatRepository = userChatRepository;
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
        }

        public async Task<IList<ChatResponse>> GetChats(int userId, int skip, int take)
        {
            List<ChatResponse> chats = new List<ChatResponse>();
            var chatIds = await _userChatRepository.GetWhereAsync(x => x.UserId == userId).ToListAsync();

            foreach (var item in chatIds)
            {
                var chat = await _chatRepository.GetWhereAsync(x => x.Id == item.ChatId).Include(x => x.Messages).FirstOrDefaultAsync();

                if (chat != null && chat.Messages.Count > 0)
                {
                  chats.Add(new ChatResponse { ChatName = chat.ChatName, MessageCount = chat.Messages.Count });//TODO
                }
            }
         
            return chats.Skip(skip).Take(take).ToList();
        }

        public async Task<IList<Message>> GetMessages(int chatId, int skip, int take)
        {
            var msg = await _messageRepository.GetWhereAsync(x => x.ChatId == chatId).ToListAsync();
            return msg.Skip(skip).Take(take).ToList();
        }

        public async Task<Chat> CreateChat(string chatName, User owner, int[] ids)
        {
            var chat = new Chat { ChatName = chatName, ChatOwner = owner  };

            var users = await GetUsersByIdList(ids);
            await _userChatRepository.AddAsync(new UserChat { Chat = chat, User = owner });

            foreach (var item in users)
            {
                if (item != null && item.Id != owner.Id) //TODO check the same values
                {
                    await _userChatRepository.AddAsync(new UserChat
                    {
                        Chat = chat,
                        User = item
                    });
                }
            }

            return chat;
        }

        public async Task<string> SendMessage(int chatId, User sender, string message)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);
            chat.Messages.Add(new Message { ChatId = chatId, Text = message, Sender = sender });
            await _chatRepository.UpdateAsync(chat);
            return message;
        }

        private async Task<IList<User>> GetUsersByIdList(int[] ids)
        {
            List<User> result = new List<User>();

            if (ids.Length > 0)
            {
                foreach (var item in ids)
                {
                    var user = await _userRepository.GetByIdAsync(item);
                    if (user != null)
                    {
                        result.Add(user);
                    }
                }
            }

            return result;
        }
    }
}
