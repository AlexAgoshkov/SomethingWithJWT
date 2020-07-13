using MySocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services.Interfaces
{
    public interface IChatService
    {
        Task<Chat> CreateChat(string chatName, User user1, User user2);

        Task<string> SendMessage(int chatId, string message);
    }
}
