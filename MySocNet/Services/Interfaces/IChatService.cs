using MySocNet.Models;
using MySocNet.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services.Interfaces
{
    public interface IChatService
    {
        Task<ChatResponse> AddNewUserToChatAsync(int chatId, int userId);

        Task<ChatResponse> RemoveUserFromChatAsync(int chatId, int userId);

        Task<ChatResponse> RemoveChatAsync(int ownerId, int chatId);

        Task<ChatResponse> EditChatAsync(int chatId, string chatName);

        Task<ChatResponse> CreateChatAsync(string chatName, User owner, int[] ids);

        Task<MessageResponse> SendMessageAsync(int chatId, User sender, string message);

        Task<IList<ChatLastResponse>> GetChatsAsync(int userId, int skip, int take);

        Task<GetNewMessageResponse> GetNewMessagesAsync(int chatId, int userId, int skip, int take);

        Task<IList<MessageResponse>> GetChatHistoryAsync(int chatId, int skip, int take);

        Task<ChatDetailsResponse> GetChatDetailsAsync(int chatId);

        Task<ChatResponse> AddImageToChatAsync(Image image, int chatId);

        Task<IList<MessageResponse>> GetUnReadMessages(int userId);

        Task ReadMessages(int userId);
    }
}
