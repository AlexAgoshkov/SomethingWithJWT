using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySocNet.Extensions;
using MySocNet.Input;
using MySocNet.Models;
using MySocNet.Services.Interfaces;
using Newtonsoft.Json;

namespace MySocNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    { 
        private readonly IRepository<User> _userRepository;
        private readonly IChatService _chatService;

        public ChatController(
            IRepository<User> repository,
            IChatService chatService
            )
        {
            _chatService = chatService;
            _userRepository = repository;
        }
       
        [HttpPost("CreateChat")]
        public async Task<IActionResult> CreateChat(InputChatCreate input)
        {
            var user1 = await HttpContext.GetAccessTokenByUserRepository(_userRepository);
            var user2 = await _userRepository.GetWhereAsync(x => x.Id == input.ReceiverId).Include(x => x.UserChats).FirstOrDefaultAsync();
            var chat = await _chatService.CreateChat(input.ChatName, user1, user2);

            return new ContentResult
            {
                StatusCode = 200,
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(chat.ChatName)
            };
        }

        [HttpPost("SendMessageToChat")]
        public async Task<IActionResult> SendMessage(int chatId, string message)
        {
            var msg = await _chatService.SendMessage(chatId, message);

            return new ContentResult
            {
                StatusCode = 200,
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(msg)
            };
        }
    }
}
