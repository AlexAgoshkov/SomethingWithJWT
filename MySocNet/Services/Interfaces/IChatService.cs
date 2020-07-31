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
        Task<Chat> AddNewUserToChatAsync(int chatId, int userId);

        Task<Chat> RemoveUserFromChatAsync(int chatId, int userId);

        Task<Chat> RemoveChatAsync(int ownerId, int chatId);

        Task<Chat> EditChatAsync(int chatId, string chatName);

        Task<Chat> CreateChatAsync(string chatName, User owner, int[] ids);

        Task<Message> SendMessageAsync(int chatId, User sender, string message);

        Task<IList<LastChatData>> GetChatsAsync(int userId, int skip, int take);

        Task<GetNewMessageResponse> GetNewMessagesAsync(int chatId, int userId, int skip, int take);

        Task<IList<MessageResponse>> GetChatHistoryAsync(int chatId, int skip, int take);

        Task<ChatDetailsResponse> GetChatDetailsAsync(int chatId);

        Task<Chat> AddImageToChatAsync(Image image, int chatId);

        Task ReadMessages(int userId, int chatId);
    }
}
