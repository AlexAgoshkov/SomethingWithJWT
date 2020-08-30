using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using MimeKit.Encodings;
using MySocNet.Enums;
using MySocNet.Exceptions;
using MySocNet.Input;
using MySocNet.Models;
using MySocNet.Response;
using MySocNet.Services.Interfaces;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MySocNet.Services
{
    public class ChatService : IChatService
    {
        private readonly IRepository<Chat> _chatRepository;
        private readonly IRepository<UserChat> _userChatRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Message> _messageRepository;
        private readonly IRepository<UserChatRead> _userMessageRepository;
        private readonly IRepository<ChatMembers> _chatMemberRepository;
        private readonly IImageService _imageService;
        private readonly ILastDataService _lastDataService;
        private readonly IMapper _mapper;

        public ChatService(
            IRepository<User> userRepository,
            IRepository<UserChat> userChatRepository,
            IRepository<Chat> chatRepository,
            IRepository<Message> messageRepository,
            IRepository<UserChatRead> userMessageRepository,
            IRepository<ChatMembers> chatMemberRepository,
            IImageService imageService,
            ILastDataService lastDataService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _userChatRepository = userChatRepository;
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
            _userMessageRepository = userMessageRepository;
            _chatMemberRepository = chatMemberRepository;
            _imageService = imageService;
            _lastDataService = lastDataService;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<Chat>> GetFiltredChatAsync(SearchChatsInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Search))
                throw new EntityNotFoundException("Search was empty");

            var query = _chatRepository
               .GetWhere(x => !x.IsPrivate && x.ChatName.ToUpper().Contains(input.Search));

            query = input.IsAscending ? query.OrderBy(x => x.ChatName) : query.OrderByDescending(x => x.ChatName);

            int totalCount = await query.CountAsync();

            var chats = await query.Skip(input.Skip).Take(input.Take).ToListAsync();

            return new PaginatedResponse<Chat>(totalCount, chats);
        }

        public async Task<List<Chat>> GetUserChatsAsync(User user, UserChatsInput input)
        {
            var userChat = _userChatRepository
                .GetWhere(x => x.User.Id == user.Id && x.IsPrivateMask == input.IsPrivateMask)
                .Select(x => x.ChatId);

            userChat = userChat.Skip(input.Skip).Take(input.Take);

            return await _chatRepository.GetWhere(x => userChat.Contains(x.Id)).ToListAsync();
        }

        public async Task<UserChat> AddToHiddenListAsync(User user, int chatId)
        {
            var userChat = await _userChatRepository
                .GetWhere(x => x.UserId == user.Id && x.ChatId == chatId).FirstOrDefaultAsync();

            userChat.IsPrivateMask = true;

            await _userChatRepository.UpdateAsync(userChat);

            return userChat;
        }

        public async Task<Chat> JoinToChannel(int channelId, User user)
        {
            var userChats = await _chatMemberRepository
                .GetWhere(x => x.ChatId == channelId || x.UserId == user.Id)
                .Include(x => x.Chat).FirstOrDefaultAsync();

            if (userChats == null || userChats.Chat.ChatType == ChatType.Chat || userChats.Chat.IsPrivate)
                throw new EntityNotFoundException("Chat not found");

            userChats.IsUserJoined = true;

            await _chatMemberRepository.UpdateAsync(userChats);

            return userChats.Chat;
        }

        public async Task<Chat> AddImageToChatAsync(Image image, int chatId)
        {
            if (image == null)
                throw new EntityNotFoundException("Image was null");
            
            var chat = await _chatRepository.GetWhere(x => x.Id == chatId)
                  .Include(x => x.ChatImage).FirstOrDefaultAsync();

            if (chat == null)
                throw new EntityNotFoundException("Chat not found");

            chat.ChatImage = image;
            await _chatRepository.UpdateAsync(chat);
            return chat;
        }

        public async Task<Chat> InviteUserToChatAsync(int chatId, int userId)
        {
            var chat = await _chatRepository.GetWhere(x => x.Id == chatId)
                .Include(x => x.UserChats).FirstOrDefaultAsync();

            if (chat == null)
                throw new EntityNotFoundException("Chat not found");

            var user = await _userRepository.FirstOrDefaultAsync(x => x.Id == userId);
            
            if (user == null)
                throw new EntityNotFoundException("User not found");

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

            if (userChat == null)
                throw new EntityNotFoundException("UserChat not found");

            await _userChatRepository.RemoveAsync(userChat);

            return userChat.Chat;
        }

        public async Task<Chat> RemoveChatAsync(int ownerId, int chatId)
        {
            var chat = await _chatRepository.GetWhere(x => x.Id == chatId)
                .Include(x => x.UserChats).Include(x => x.Messages).FirstOrDefaultAsync();

            if (chat == null)
                throw new EntityNotFoundException("Chat not found");

            if (chat.ChatOwnerId == ownerId)
            {
                await _chatRepository.RemoveAsync(chat);
            }

            return chat;
        }

        public async Task<Chat> EditChatAsync(int chatId, string chatName)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);

            if (chat == null)
                throw new EntityNotFoundException("Chat not found");

            if (chat != null && !string.IsNullOrWhiteSpace(chatName))
            {
                chat.ChatName = chatName;
                await _chatRepository.UpdateAsync(chat);
            }
            return chat;
        }

        public async Task<ChatDetailsResponse> GetChatDetailsAsync(int chatId)
        {
            var chat = await _chatRepository.GetWhere(x => x.Id == chatId)
                .Include(x => x.UserChats).ThenInclude(x => x.User)
                .Include(x => x.ChatImage)
                .FirstOrDefaultAsync();

            if (chat == null)
                throw new EntityNotFoundException("Chat not found");

            return new ChatDetailsResponse
            {
                ChatName = chat.ChatName,
                Image = await _imageService.DownloadAsync(chat.ChatImage.CroppedImagePath),
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
                throw new EntityNotFoundException("UserMessage is not Found");

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

        public async Task ReadMessages(int userId, int chatId)
        {
            var messages = await _userMessageRepository
                .GetWhere(x => x.UserId == userId && x.ChatId == chatId && !x.IsRead)
                .ToListAsync();

            if (messages == null)
                throw new EntityNotFoundException("Messages not found");

            foreach (var item in messages)
            {
               item.IsRead = true;
               await _userMessageRepository.UpdateAsync(item);
            }
        }

        public async Task<Chat> CreateChatAsync(CreateChatInput input, User owner)
        {
            if (input.ChatType == ChatType.Channel)
                input.IsReadOnly = true;

            var chat = _mapper.Map<Chat>(input);
            chat.ChatOwner = owner;
           
            await _userChatRepository.AddAsync(new UserChat { Chat = chat, User = owner });

            var users = await _userRepository.GetWhere(x => input.Ids.Distinct().Contains(x.Id))
                .Where(x => x.Id != owner.Id).ToListAsync();
          
            await _userChatRepository.AddRangeAsync(users.Select(x => new UserChat { Chat = chat, User = x }));
            return chat;
        }

        public async Task<Message> ForwardMessageAsync(User user, int messageId, int chatId)
        {
            var chat = await _chatRepository.GetWhere(x => x.Id == chatId).FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Chat not found");
            var message = await _messageRepository.GetWhere(x => x.Id == messageId).FirstOrDefaultAsync()
                ?? throw new EntityNotFoundException("Message not found");

            var newMessage = new Message
            {
                Chat = chat,
                Sender = user,
                Time = DateTime.Now,
                OriginalMessage = message
            };

            await _messageRepository.AddAsync(newMessage);

            return newMessage;
        }

        public async Task<Message> SendMessageAsync(User user, SendMessageInput input)
        {
            var chat = await _chatRepository.GetByIdAsync(input.ChatId);
           
            if (chat == null)
                throw new EntityNotFoundException("Chat not found");

            var userChats = await _userChatRepository
                .GetWhere(x => x.ChatId == input.ChatId && user.Id == x.UserId).FirstOrDefaultAsync();

            var users = await _userChatRepository.GetWhere(x => x.ChatId == input.ChatId)
                .Select(x => x.UserId).ToListAsync();

            if (chat.IsReadOnly && user.Id != chat.ChatOwnerId)
                throw new EntityNotFoundException("You can't send messages to the Chat");

            var responseMessage = new Message
            {
                ChatId = input.ChatId,
                Text = input.Message,
                Sender = user,
                Time = DateTime.Now
            };

            if (input.Image != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var image = await _imageService.UploadAsync(input.Image, input.Filters, fileName);
                responseMessage.MessageImage = image;
            }
            
            chat.Messages.Add(responseMessage);

            await _lastDataService.AddLastChatData(_mapper.Map<LastChatData>(responseMessage));
            await _chatRepository.UpdateAsync(chat);
            await CreateUnReadMessages(users, chat); 
            return responseMessage;
        }

        private async Task CreateUnReadMessages(List<int> users, Chat chat)
        {
            var chatRep = await _userMessageRepository.GetWhere(x => x.ChatId == chat.Id).ToListAsync();

            if (chatRep == null)
            {
                await _userMessageRepository.AddRangeAsync(users.Select(x => new UserChatRead { UserId = x, Chat = chat, IsRead = false }));
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
