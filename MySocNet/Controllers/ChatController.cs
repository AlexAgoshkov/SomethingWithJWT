using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class ChatController : ApiControllerBase
    { 
        private readonly IRepository<User> _userRepository;
        private readonly IChatService _chatService;
        private readonly IMapper _mapper;

        public ChatController(
            IRepository<User> userRepository,
            IChatService chatService) : base(userRepository)
        {
            _chatService = chatService;
            _userRepository = userRepository;
        }

        [HttpGet("ShowChats")]
        public async Task<IEnumerable<ChatResponse>> GetChats([FromQuery]PaginatedInput input)
        {
            var user = await CurrentUser();
            return await _chatService.GetChats(user.Id, input.Skip, input.Take);
        }

        [HttpPost("CreateChat")]
        public async Task<IActionResult> CreateChat(InputChatCreate input)
        {
            var chatOwner = await HttpContext.GetUserByAccessTokenAsync(_userRepository);
            var chat = await _chatService.CreateChat(input.ChatName, chatOwner, input.Ids);
            return JsonResult(chat);
        }

        [HttpGet("ShowMessages")]
        public async Task<IActionResult> GetChatMessages([FromQuery]GetMessagesInput input)
        {
            var messages = await _chatService.GetMessages(input.ChatId, input.Skip, input.Take);
            var user = await CurrentUser();
            await _chatService.GetReadMessage(user.Id, input.ChatId);
            return JsonResult(messages);
        }

        [HttpPost("SendMessageToChat")]
        public async Task<IActionResult> SendMessage(SendMessageInput input)
        {
            var messageSender = await HttpContext.GetUserByAccessTokenAsync(_userRepository);
            var message = await _chatService.SendMessage(input.ChatId, messageSender, input.Message);
            return JsonResult(message);
        }
    }
}
