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
        private readonly ILogger<ChatController> _logger;
        private readonly IMapper _mapper;

        public ChatController(
            IRepository<User> userRepository,
            IChatService chatService,
            ILogger<ChatController> logger,
            IMapper mapper
            ) : base(userRepository)
        {
            _chatService = chatService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("AddNewUserToChat")]
        public async Task<IActionResult> AddNewUserToChat(UserToChatInput input)
        {
            try
            {
                var response = await _chatService.AddNewUserToChatAsync(input.ChatId, input.UserId);
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
                var response = await _chatService.RemoveUserFromChatAsync(input.ChatId, input.UserId);
                return JsonResult(_mapper.Map<ChatResponse>(response));
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
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
                return JsonResult(_mapper.Map<ChatResponse>(chat));
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }

        [HttpPost("EditChat")]
        public async Task<IActionResult> EditChat(UpdateChatInput input)
        {
            try
            {
                var response = await _chatService.EditChatAsync(input.ChatId, input.ChatName);
                return JsonResult(_mapper.Map<ChatResponse>(response));
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("GetChatDetails")]
        public async Task<IActionResult> GetChatDetails(int chatId)
        {
            try
            {
                var result = await _chatService.GetChatDetailsAsync(chatId);
                return JsonResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
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
                return JsonResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }

        [HttpPost("CreateChat")]
        public async Task<IActionResult> CreateChat(InputChatCreate input)
        {
            try
            {
                var chatOwner = await CurrentUser();
                var chat = await _chatService.CreateChatAsync(input.ChatName, chatOwner, input.Ids);
                return JsonResult(chat);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("GetAllMessages")]
        public async Task<IActionResult> GetAllMessages([FromQuery] GetMessagesInput input)
        {
            try
            {
                var messages = await _chatService.GetChatHistoryAsync(input.ChatId, input.Skip, input.Take);
                return JsonResult(messages);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("GetNewMessages")]
        public async Task<IActionResult> GetNewMessages([FromQuery]GetMessagesInput input)
        {
            var user = await CurrentUser();
            var messages = await _chatService.GetNewMessagesAsync(input.ChatId, user.Id, input.Skip, input.Take);
            return Ok(messages);
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
