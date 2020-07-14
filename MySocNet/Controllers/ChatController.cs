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
using MySocNet.InputData;
using MySocNet.Models;
using MySocNet.Response;
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
            IChatService chatService)
        {
            _chatService = chatService;
            _userRepository = repository;
        }

        [HttpGet("ShowChats")]
        public async Task<IEnumerable<ChatResponse>> GetChats(PaginatedInput input)
        {
            var user = await HttpContext.GetAccessTokenByUserRepository(_userRepository);
            return await _chatService.GetChats(user.Id, input.Skip, input.Take);
        }

        [HttpPost("CreateChat")]
        public async Task<IActionResult> CreateChat(InputChatCreate input)
        {
            var chatOwner = await HttpContext.GetAccessTokenByUserRepository(_userRepository); //chat owner
            await _chatService.CreateChat(input.ChatName, chatOwner, input.Ids);

            return new ContentResult
            {
                StatusCode = 200,
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(input)
            };
        }

        [HttpPost("ShowMessages")]
        public async Task<IEnumerable<Message>> GetChatMessages(GetMessagesInput input)
        {
            return await _chatService.GetMessages(input.ChatId, input.Skip, input.Take);
        }

        [HttpPost("SendMessageToChat")]
        public async Task<IActionResult> SendMessage(SendMessageInput input)
        {
            var messageSender = await HttpContext.GetAccessTokenByUserRepository(_userRepository);
          
            var msg = await _chatService.SendMessage(input.ChatId, messageSender, input.Message);

            return new ContentResult
            {
                StatusCode = 200,
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(msg)
            };
        }
    }
}
