using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySocNet.Extensions;
using MySocNet.Hubs;
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
        private readonly IChatService _chatService;
        private readonly IMapper _mapper;
        private readonly ILogger<ChatController> _logger;

        public ChatController(
            IRepository<User> userRepository,
            IChatService chatService,
            ILogger<ChatController> logger,
            IMapper mapper
            ) : base(userRepository)
        {
            _logger = logger;
            _chatService = chatService;
            _mapper = mapper;
           
        }

        [HttpPost("AddNewUserToChat")]
        public async Task<IActionResult> AddNewUserToChat(UserToChatInput input)
        {
            try
            {
                var user = await CurrentUser();
                var response = await _chatService.AddNewUserToChatAsync(input.ChatId, input.UserId);
                _logger.LogInformation($"User id: {user.Id} Login {user.UserName} Added User id: {input.UserId} to Chat Id {input.ChatId}");
                return JsonResult(_mapper.Map<ChatResponse>(response));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("RemoveUserFromChat")]
        public async Task<IActionResult> RemoveUserFromChat(UserToChatInput input)
        {
            try
            {
                var user = await CurrentUser();
                var response = await _chatService.RemoveUserFromChatAsync(input.ChatId, input.UserId);
                _logger.LogInformation($"User id: {user.Id} Login {user.UserName} Removed User Id {input.UserId} from Chat Id {input.ChatId}");
                return JsonResult(_mapper.Map<ChatResponse>(response));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Policy = Policies.User)]
        [HttpDelete("RemoveChat")]
        public async Task<IActionResult> RemoveChat(int chatId)
        {
            try
            {
                var user = await CurrentUser();
                var chat = await _chatService.RemoveChatAsync(user.Id, chatId);
                _logger.LogInformation($"User id: {user.Id} Login {user.UserName} Removed Chat Id: {chatId}");
                return JsonResult(_mapper.Map<ChatResponse>(chat));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("EditChat")]
        public async Task<IActionResult> EditChat(UpdateChatInput input)
        {
            try
            {
                var user = await CurrentUser();
                var response = await _chatService.EditChatAsync(input.ChatId, input.ChatName);
                _logger.LogInformation($"User id: {user.Id} Login {user.UserName} Edited Chat Id {input.ChatId}");
                return JsonResult(_mapper.Map<ChatResponse>(response));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetChatDetails")]
        public async Task<IActionResult> GetChatDetails(int chatId)
        {
            try
            {
                var user = await CurrentUser();
                var result = await _chatService.GetChatDetailsAsync(chatId);
                _logger.LogInformation($"User id: {user.Id} Login {user.UserName} Got ChatDetails from Chat Id: {chatId}");
                return JsonResult(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetChats")]
        public async Task<IActionResult> GetChats([FromQuery]PaginatedInput input)
        {
            try
            {
                var user = await CurrentUser();
                var chatList = await _chatService.GetChatsAsync(user.Id, input.Skip, input.Take);
                var response = new GetChatsResponse { ChatResponses = chatList, TotalCount = chatList.Count };
                _logger.LogInformation($"User id: {user.Id} Login {user.UserName} Got his/her Chats total count {chatList.Count}");
                return JsonResult(response);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("CreateChat")]
        public async Task<IActionResult> CreateChat(InputChatCreate input)
        {
            try
            {
                var chatOwner = await CurrentUser();
                var chat = await _chatService.CreateChatAsync(input.ChatName, chatOwner, input.Ids);
                _logger.LogInformation($"User id: {chatOwner.Id} Login {chatOwner.UserName} Created new Chat Id: {chat.Id}");
                return JsonResult(chat);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetAllMessages")]
        public async Task<IActionResult> GetAllMessages([FromQuery] GetMessagesInput input)
        {
            try
            {
                var user = await CurrentUser();
                var messages = await _chatService.GetChatHistoryAsync(input.ChatId, input.Skip, input.Take);
                _logger.LogInformation($"User id: {user.Id} Login {user.UserName} Read Messages from Chat Id {input.ChatId}");
                return JsonResult(messages);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetNewMessages")]
        public async Task<IActionResult> GetNewMessages([FromQuery]GetMessagesInput input)
        {
            try
            {
                var user = await CurrentUser();
                var messages = await _chatService.GetNewMessagesAsync(input.ChatId, user.Id, input.Skip, input.Take);
                _logger.LogInformation($"User id: {user.Id} Login {user.UserName} Read New Messages from Chat Id {input.ChatId}");
                return JsonResult(messages);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("SendMessageToChat")]
        public async Task<IActionResult> SendMessage(SendMessageInput input)
        {
            try
            {
                var messageSender = await CurrentUser();
                var message = await _chatService.SendMessageAsync(input.ChatId, messageSender, input.Message);
                _logger.LogInformation($"User id: {messageSender.Id}, Login {messageSender.UserName} Sent message to chat Id: {input.ChatId}");
                return JsonResult(message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
