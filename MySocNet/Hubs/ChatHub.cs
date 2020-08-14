using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MySocNet.Extensions;
using MySocNet.Input;
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
        private readonly IChatService _chatService;  

        public ChatHub(IRepository<User> userReposity, 
             IChatService chatService
            )
        {
            _userRepository = userReposity;
            _chatService = chatService;
        }

        public async Task SendMessage(int chatId, string message)
        {
            var context = Context.GetHttpContext();
            var token = await context.GetAccessToken();
            var user = await _userRepository.GetWhere(x => x.Authentication.AccessToken == token)
                .Include(x => x.Authentication).FirstOrDefaultAsync();
            var input = new SendMessageInput { ChatId = chatId, Message = message };
            await _chatService.SendMessageAsync(user, input);
            var userName = $"{user.FirstName} {user.SurName}";
            await Clients.All.SendAsync("ReceiveMessage",chatId, userName, message);
        }
       
        public async Task Notify(string userName, string message)
        {
            await Clients.All.SendAsync("Receive", message, userName);
        }
    }
}
