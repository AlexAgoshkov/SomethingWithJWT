using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("AddNewUserToChat")]
        public async Task<IActionResult> AddNewUserToChat(UserToChatInput input)
        {
            var response = await _chatService.AddNewUserToChatAsync(input.ChatId, input.UserId);
            return JsonResult(response);
        }

        [HttpDelete("RemoveUserFromChat")]
        public async Task<IActionResult> RemoveUserFromChat(UserToChatInput input)
        {
            var response = await _chatService.RemoveUserFromChatAsync(input.ChatId, input.UserId);
            return JsonResult(response);
        }

        [Authorize(Policy = Policies.User)]
        [HttpDelete("RemoveChat")]
        public async Task<IActionResult> RemoveChat(int chatId)
        {
            var user = await CurrentUser();
            await _chatService.RemoveChatAsync(user.Id, chatId);
            return Ok();
        }

        [HttpPost("EditChat")]
        public async Task<IActionResult> EditChat(UpdateChatInput input)
        {
            var response = await _chatService.EditChatAsync(input.ChatId, input.ChatName);
            return JsonResult(response);
        }

        [HttpGet("GetChatDetails")]
        public async Task<IActionResult> GetChatDetails(int chatId)
        {
            var result = await _chatService.GetChatDetailsAsync(chatId);
            return JsonResult(result);
        }

        [HttpGet("GetChats")]
        public async Task<IEnumerable<ChatResponse>> GetChats([FromQuery]PaginatedInput input)
        {
            var user = await CurrentUser();
            return await _chatService.GetChatsAsync(user.Id, input.Skip, input.Take);
        }

        [HttpPost("CreateChat")]
        public async Task<IActionResult> CreateChat(InputChatCreate input)
        {
            var chatOwner = await CurrentUser();
            var chat = await _chatService.CreateChatAsync(input.ChatName, chatOwner, input.Ids);
            return JsonResult(chat);
        }

        [HttpGet("GetAllMessages")]
        public async Task<IActionResult> GetAllMessages([FromQuery] GetMessagesInput input)
        {
            var messages = await _chatService.GetChatHistoryAsync(input.ChatId, input.Skip, input.Take);
            return JsonResult(messages);
        }

        [HttpGet("GetNewMessages")]
        public async Task<IActionResult> GetNewMessages([FromQuery]GetMessagesInput input)
        {
            var messages = await _chatService.GetNewMessagesAsync(input.ChatId, input.Skip, input.Take);
            var user = await CurrentUser();
            await _chatService.GetReadMessageAsync(user.Id, input.ChatId);
            return JsonResult(messages);
        }

        [HttpPost("SendMessageToChat")]
        public async Task<IActionResult> SendMessage(SendMessageInput input)
        {
            var messageSender = await CurrentUser();
            var message = await _chatService.SendMessageAsync(input.ChatId, messageSender, input.Message);
            return JsonResult(message);
        }
    }
}
