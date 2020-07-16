﻿using MySocNet.Models;
using MySocNet.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services.Interfaces
{
    public interface IChatService
    {
        Task<Chat> AddNewUserToChatAsync(int chatId, int userId);

        Task<Chat> RemoveUserFromChatAsync(int chatId, int userId);

        Task RemoveChatAsync(int ownerId, int chatId);

        Task<Chat> EditChatAsync(int chatId, string chatName);

        Task<Chat> CreateChatAsync(string chatName, User owner, int[] ids);

        Task<Message> SendMessageAsync(int chatId, User sender, string message);

        Task<IList<ChatResponse>> GetChatsAsync(int userId, int skip, int take);

        Task<IList<Message>> GetNewMessagesAsync(int chatId, int skip, int take);

        Task<IList<Message>> GetChatHistoryAsync(int chatId, int skip, int take);

        Task GetReadMessageAsync(int userId, int chatId);

        Task<ChatDetailsResponse> GetChatDetailsAsync(int chatId);
    }
}
