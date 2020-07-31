using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MimeKit.Encodings;
using MySocNet.Models;
using MySocNet.Response;
using MySocNet.Services.Interfaces;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IRepository<UserMessage> _userMessageRepository;
        private readonly IImageService _imageService;
        private readonly ILastDataService _lastDataService;
        private readonly IMapper _mapper;

        public ChatService(
            IRepository<User> userRepository,
            IRepository<UserChat> userChatRepository,
            IRepository<Chat> chatRepository,
            IRepository<Message> messageRepository,
            IRepository<UserMessage> userMessageRepository,
            IImageService imageService,
            ILastDataService lastDataService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _userChatRepository = userChatRepository;
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
            _userMessageRepository = userMessageRepository;
            _imageService = imageService;
            _lastDataService = lastDataService;
            _mapper = mapper;
        }

        public async Task<Chat> AddImageToChatAsync(Image image, int chatId)
        {
            var chat = await _chatRepository.GetWhere(x => x.Id == chatId)
                  .Include(x => x.ChatImage).FirstOrDefaultAsync();
            chat.ChatImage = image;
            await _chatRepository.UpdateAsync(chat);
            return chat;
        }

        public async Task<Chat> AddNewUserToChatAsync(int chatId, int userId)
        {
            var chat = await _chatRepository.GetWhere(x => x.Id == chatId)
                .Include(x => x.UserChats).FirstOrDefaultAsync();

            if (chat == null)
                throw new ArgumentException("Chat not found");

            var user = await _userRepository.FirstOrDefaultAsync(x => x.Id == userId);
            // DONE: check global exception handler. correct status codes must be returned
            if (user == null)
                throw new ArgumentException("User not found");

            chat.UserChats.Add(new UserChat { ChatId = chatId, UserId = userId });

            await _chatRepository.UpdateAsync(chat);

            return chat;
        }

        public async Task<Chat> RemoveUserFromChatAsync(int chatId, int userId)
        {
            var userChat = await _userChatRepository
                .GetWhere(x => x.ChatId == chatId && x.UserId == userId)
                .Include(x => x.Chat)
                .FirstOrDefaultAsync();

            // DONE: check fot null
            if (userChat == null)
                throw new Exception("UserChat not found");

            await _userChatRepository.RemoveAsync(userChat);
            return userChat.Chat;
        }

        public async Task<Chat> RemoveChatAsync(int ownerId, int chatId)
        {
            var chat = await _chatRepository.GetWhere(x => x.Id == chatId)
                .Include(x => x.UserChats).Include(x => x.Messages).FirstOrDefaultAsync();

            // DONE: check for null
            if (chat == null)
                throw new Exception("Chat not found");
            if (chat.ChatOwnerId == ownerId)
            {
                await _chatRepository.RemoveAsync(chat);
            }

            return chat;
        }

        public async Task<Chat> EditChatAsync(int chatId, string chatName)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);

            // DONE: check for null
            if (chat == null)
                throw new Exception("Chat not found");

            if (chat != null && !string.IsNullOrWhiteSpace(chatName))
            {
                chat.ChatName = chatName;
                await _chatRepository.UpdateAsync(chat);
            }
            return chat;
        }

        public async Task<IList<LastChatData>> GetChatsAsync(int userId, int skip, int take)
        {
            // DONE: configure and test mapping; remove foreach
            return await _lastDataService.GetLastData(userId);
        }

        // DONE: refactoring
        public async Task<ChatDetailsResponse> GetChatDetailsAsync(int chatId)
        {
            var chat = await _chatRepository.GetWhere(x => x.Id == chatId)
                .Include(x => x.UserChats).ThenInclude(x => x.User)
                .Include(x => x.ChatImage)
                .FirstOrDefaultAsync();

            if (chat == null)
                throw new Exception("Chat not found");

            return new ChatDetailsResponse
            {
                ChatName = chat.ChatName,
                Image = await _imageService.DownloadAsync(chat.ChatImage.ImagePath),
                Users = await _userRepository
                    .GetWhere(x => chat.UserChats.Select(y => y.UserId)
                    .Contains(x.Id))
                    .ToListAsync()
            };
        }

        public async Task<IList<MessageResponse>> GetChatHistoryAsync(int chatId, int skip, int take)
        {
            return _mapper.Map<List<MessageResponse>>(await _messageRepository
                .GetWhere(x => x.ChatId == chatId)
                .Skip(skip).Take(take).ToListAsync());
        }

        public async Task<GetNewMessageResponse> GetNewMessagesAsync(int chatId, int userId, int skip, int take)
        {
            var userMessage = await _userMessageRepository.GetWhere(x => x.UserId == userId && x.ChatId == chatId && !x.IsRead)
                .Select(x => x.ChatId).ToListAsync();

            if (userMessage == null)
                throw new Exception("UserMessage is not Found");

            var query = _messageRepository
                .GetWhere(x => userMessage.Contains(x.Id));
            
            var totalCount = await query.CountAsync();

            var result = await query
                .OrderByDescending(x => x.Time)
                .Skip(skip).Take(take)
                .ToListAsync();

            await ReadMessages(userId, chatId);

            var messageResponse = _mapper.Map<List<MessageResponse>>(result);
            return new GetNewMessageResponse(messageResponse, totalCount);
        }

        public async Task ReadMessages(int userId, int chatId) //------------------------------------------
        {
            var messages = await _userMessageRepository.GetWhere(x => x.UserId == userId && x.ChatId == chatId && !x.IsRead)
                .ToListAsync();

            foreach (var item in messages)
            {
                item.IsRead = true;
                await _userMessageRepository.UpdateAsync(item);
            }
        }

        public async Task<Chat> CreateChatAsync(string chatName, User owner, int[] usersIds)
        {
            var chat = new Chat { ChatName = chatName, ChatOwner = owner };

            await _userChatRepository.AddAsync(new UserChat { Chat = chat, User = owner });

            var users = await _userRepository.GetWhere(x => usersIds.Distinct().Contains(x.Id))
                .Where(x => x.Id != owner.Id).ToListAsync();
          
            await _userChatRepository.AddRangeAsync(users.Select(x => new UserChat { User = x, Chat = chat }));
            return chat;
        }

        public async Task<Message> SendMessageAsync(int chatId, User sender, string message)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);
           
            if (chat == null)
                throw new Exception("User not found");

            var users = await _userChatRepository.GetWhere(x => x.ChatId == chatId)
                .Select(x => x.UserId).ToListAsync();

            var responseMessage = new Message
            {
                ChatId = chatId,
                Text = message,
                Sender = sender,
                Time = DateTime.Now
            };
            chat.Messages.Add(responseMessage);

            // TODO: use mapper instead new { }--------------------------
            // _mapper.Map<LastChatData>(responseMessage)

            await _lastDataService.AddLastChatData(new LastChatData
            {
                ChatId = chat.Id,
                UserName = $"{sender.FirstName} {sender.SurName}",
                Text = message,
            });

            await _chatRepository.UpdateAsync(chat);
            await CreateUnReadMessages(users, chat); 
            return responseMessage;
        }

        private async Task CreateUnReadMessages(List<int> users, Chat chat)
        {
            var chatRep = await _userMessageRepository.GetWhere(x => x.ChatId == chat.Id).ToListAsync();

            if (chatRep == null)
            {
                await _userMessageRepository.AddRangeAsync(users.Select(x => new UserMessage { UserId = x, Chat = chat, IsRead = false }));
            }
            else
            {
                var ids = chatRep.Select(x => x.UserId);
                var test = chatRep.Where(x => ids.Contains(x.UserId) && x.ChatId == chat.Id);
                foreach (var item in test)
                {
                   await _userMessageRepository.UpdateAsync(item);
                }
            }
        }
    }
}
