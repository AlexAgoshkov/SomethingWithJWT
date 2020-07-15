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
        Task<Chat> CreateChat(string chatName, User owner, int[] ids);

        Task<Message> SendMessage(int chatId, User sender, string message);

        Task<IList<ChatResponse>> GetChats(int userId, int skip, int take);

        Task<IList<Message>> GetMessages(int chatId, int skip, int take);

        Task GetReadMessage(int userId, int chatId);
    }
}
