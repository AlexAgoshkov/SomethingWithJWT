using Microsoft.EntityFrameworkCore;
using MySocNet.Models;
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

        public ChatService(IRepository<User> userRepository, IRepository<UserChat> userChatRepository, IRepository<Chat> chatRepository)
        {
            _userRepository = userRepository;
            _userChatRepository = userChatRepository;
            _chatRepository = chatRepository;
        }

        public async Task<Chat> CreateChat(string chatName, User user1, User user2)
        {
            var chat = new Chat { ChatName = chatName };

            try
            {
                await _chatRepository.AddAsync(chat);

                await _userChatRepository.AddAsync(new UserChat
                {
                    Chat = chat,
                    User = user1
                });

                await _userChatRepository.AddAsync(new UserChat
                {
                    Chat = chat,
                    User = user2
                });
            }
            catch (Exception ex)
            {

            }

            return chat;
        }

        public async Task<string> SendMessage(int chatId, string message)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);
            chat.Messages.Add(new Message { ChatId = chatId, Text = message });
            await _chatRepository.UpdateAsync(chat);
            return message;
        }
    }
}
