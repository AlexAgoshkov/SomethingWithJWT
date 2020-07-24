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

        public async Task<ChatResponse> AddImageToChatAsync(Image image, int chatId)
        {
            var chat = await _chatRepository.GetWhere(x => x.Id == chatId)
                  .Include(x => x.ChatImage).FirstOrDefaultAsync();
            chat.ChatImage = image;
            await _chatRepository.UpdateAsync(chat);
            return _mapper.Map<ChatResponse>(chat);
        }

        public async Task<ChatResponse> AddNewUserToChatAsync(int chatId, int userId)
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

            return _mapper.Map<ChatResponse>(chat);
        }

        public async Task<ChatResponse> RemoveUserFromChatAsync(int chatId, int userId)
        {
            var userChat = await _userChatRepository
                .GetWhere(x => x.ChatId == chatId && x.UserId == userId)
                .Include(x => x.Chat)
                .FirstOrDefaultAsync();

            // DONE: check fot null
            if (userChat == null)
                throw new Exception("UserChat not found");

            await _userChatRepository.RemoveAsync(userChat);
            return _mapper.Map<ChatResponse>(userChat.Chat);
        }

        public async Task<ChatResponse> RemoveChatAsync(int ownerId, int chatId)
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

            return _mapper.Map<ChatResponse>(chat);
        }

        public async Task<ChatResponse> EditChatAsync(int chatId, string chatName)
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
            return _mapper.Map<ChatResponse>(chat);
        }

        public async Task<IList<ChatLastResponse>> GetChatsAsync(int userId, int skip, int take)
        {
            // DONE: configure and test mapping; remove foreach
            return _mapper.Map<List<ChatLastResponse>>(await _lastDataService.GetLastData(userId));
        }

        // DONE: refactoring
        public async Task<ChatDetailsResponse> GetChatDetailsAsync(int chatId)
        {
            List<User> list = new List<User>();
            var result = new ChatDetailsResponse();

            var chat = await _chatRepository.GetWhere(x => x.Id == chatId)
                .Include(x => x.UserChats).ThenInclude(x => x.User).Include(x => x.ChatImage).FirstOrDefaultAsync();

            if (chat == null)
                throw new Exception("Chat not found");

            var users = await _userRepository.GetWhere(x => chat.UserChats.Select(y => y.UserId)
                .Contains(x.Id)).ToListAsync();

            result.ChatName = chat.ChatName;
            result.Image = await _imageService.DownloadAsync(chat.ChatImage.ImagePath);
              
            foreach (var item in users)
            {
                list.Add(item);
            }
            result.Users = list;
            
            return result;
        }

        public async Task<IList<MessageResponse>> GetChatHistoryAsync(int chatId, int skip, int take)
        {
            return _mapper.Map<List<MessageResponse>>(await _messageRepository
                .GetWhere(x => x.ChatId == chatId)
                .Skip(skip).Take(take).ToListAsync());
        }

        public async Task<GetNewMessageResponse> GetNewMessagesAsync(int chatId, int userId, int skip, int take)
        {
            var userMes = await _userMessageRepository.GetWhere(x => x.UserId == userId && !x.IsRead)
                .Select(x => x.MessageId).ToListAsync();

            var query = _messageRepository
                .GetWhere(x =>  userMes.Contains(x.Id) && x.ChatId == chatId);

            var totalCount = await query.CountAsync();

            var result = await query
                .OrderByDescending(x => x.Time)
                .Skip(skip).Take(take)
                .ToListAsync();

            // DONE: create endpoint for mark message as read
            var messageResponse = _mapper.Map<List<MessageResponse>>(result);
            return new GetNewMessageResponse(messageResponse, totalCount);
        }

        public async Task<IList<MessageResponse>> GetUnReadMessages(int userId)
        {
            return _mapper.Map<IList<MessageResponse>>(await _userMessageRepository.GetWhere(x => x.UserId == userId && !x.IsRead)
                .Include(x => x.Message).Select(x => x.Message).ToListAsync());
        }
        public async Task ReadMessages(int userId) //------------------------------------------
        {
            var test = await _userMessageRepository.GetWhere(x => x.UserId == userId && !x.IsRead)
                .ToListAsync();

            foreach (var item in test)
            {
                item.IsRead = true;
                await _userMessageRepository.UpdateAsync(item);
            }
        }

        public async Task<ChatResponse> CreateChatAsync(string chatName, User owner, int[] usersIds)
        {
            var chat = new Chat { ChatName = chatName, ChatOwner = owner };

            await _userChatRepository.AddAsync(new UserChat { Chat = chat, User = owner });

            var users = await _userRepository.GetWhere(x => usersIds.Distinct().Contains(x.Id))
                .Where(x => x.Id != owner.Id).ToListAsync();
            // DONE: think about it
            List<UserChat> list = new List<UserChat>();
            foreach (var item in users)
            {
                list.Add(new UserChat { User = item, Chat = chat});
            }

            await _userChatRepository.AddRangeAsync(list);
            return _mapper.Map<ChatResponse>(chat);
        }

        public async Task<MessageResponse> SendMessageAsync(int chatId, User sender, string message)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);
            var users = await _userChatRepository.GetWhere(x => x.ChatId == chatId)
                .Select(x => x.UserId).ToListAsync();

            // DONE: throw ex
            if (chat == null)
                throw new Exception("User not found");

            var responseMessage = new Message
            {
                ChatId = chatId,
                Text = message,
                Sender = sender,

                Time = DateTime.Now
            };
            chat.Messages.Add(responseMessage);

            // DONE: use mapper instead new { }----------
          
            await _lastDataService.AddLastChatData(new LastChatData
            {
                ChatId = chat.Id,
                UserName = $"{sender.FirstName} {sender.SurName}",
                Text = message,
            });

            await _chatRepository.UpdateAsync(chat);
            await CreateUnReadMessages(users, responseMessage); //Add data to UserMessage Table

            return _mapper.Map<MessageResponse>(responseMessage);
        }

        private async Task CreateUnReadMessages(List<int> users, Message responseMessage)
        {
            List<UserMessage> list = new List<UserMessage>();
            foreach (var item in users)
            {
                list.Add(new UserMessage { UserId = item, MessageId = responseMessage.Id, IsRead = false });
            }
            await _userMessageRepository.AddRangeAsync(list);
        }

        private async Task<IList<User>> GetUsersByIdListAsync(IEnumerable<int> ids)
        {
            // DONE: rewrite with LINQ
            return await _userRepository.GetWhere(x => ids.Contains(x.Id)).ToListAsync();
        }
    }
}
