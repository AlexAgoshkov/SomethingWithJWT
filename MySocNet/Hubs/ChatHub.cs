using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MySocNet.Extensions;
using MySocNet.Models;
using MySocNet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Chat> _chatRepository;
        private readonly IChatService _chatService;  

        public ChatHub(IRepository<User> userReposity, 
            IRepository<Chat> chatRepository,
             IChatService chatService)
        {
            _userRepository = userReposity;
            _chatRepository = chatRepository;
            _chatService = chatService;
        }

        public async Task SendMessage(int chatId, string token, string message)
        {
            var test = await _userRepository.GetWhere(x => x.Authentication.AccessToken == token)
                .Include(x => x.Authentication).FirstOrDefaultAsync();
            await _chatService.SendMessageAsync(chatId, test, message);
            await Clients.All.SendAsync("ReceiveMessage", chatId, test.FirstName, message);
        }
    }
}
