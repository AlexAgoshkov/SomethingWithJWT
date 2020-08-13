using MySocNet.Input;
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
        Task<Chat> InviteUserToChatAsync(int chatId, int userId);

        Task<PaginatedResponse<Chat>> GetFiltredChatAsync(SearchChatsInput input);

        Task<Chat> RemoveUserFromChatAsync(int chatId, int userId);

        Task<Chat> RemoveChatAsync(int ownerId, int chatId);

        Task<Chat> EditChatAsync(int chatId, string chatName);

        Task<Chat> CreateChatAsync(InputChatCreate input, User chatOwner);
        Task<Message> SendMessageAsync(User user, SendMessageInput input);
        Task<GetNewMessageResponse> GetNewMessagesAsync(int chatId, int userId, int skip, int take);

        Task<IList<MessageResponse>> GetChatHistoryAsync(int chatId, int skip, int take);

        Task<ChatDetailsResponse> GetChatDetailsAsync(int chatId);

        Task<Chat> AddImageToChatAsync(Image image, int chatId);

        Task ReadMessages(int userId, int chatId);

        Task<Chat> JoinToChannel(int channelId, User user);

        Task<List<Chat>> GetUserChatsAsync(User user, UserChatsInput input);

        Task<UserChat> AddToHiddenListAsync(User user, int chatId);

        Task<Message> ForwardMessageAsync(User user, int messageId, int chatId);
    }
}
