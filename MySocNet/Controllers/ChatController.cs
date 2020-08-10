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
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace MySocNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ChatController : ApiControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IMapper _mapper;
        private readonly IMyLogger _myLogger;
        private readonly IRepository<Chat> _chatRepository;

        public ChatController(
            IRepository<User> userRepository,
            IRepository<Chat> chatRepository,
            IChatService chatService,
            IMyLogger myLogger,
            IMapper mapper
            ) : base(userRepository)
        {
            _chatService = chatService;
            _chatRepository = chatRepository;
            _mapper = mapper;
        }

        [HttpPost("InviteUserToChat")]
        public async Task<IActionResult> InviteUserToChat(UserToChatInput input)
        {
            var user = await CurrentUser();
            var response = await _chatService.InviteUserToChatAsync(input.ChatId, input.UserId);
            return JsonResult(_mapper.Map<ChatResponse>(response));
        }

        [Authorize(Policy = Policies.User)]
        [HttpPost("JoinToChannel")]
        public async Task<IActionResult> JoinToChannel(int channelId)
        {
            var currentUser = await CurrentUser();
            var channel = await _chatService.JoinToChannel(channelId, currentUser);
            return JsonResult(channel);
        }

        [HttpDelete("RemoveUserFromChat")]
        public async Task<IActionResult> RemoveUserFromChat(UserToChatInput input)
        {
            var user = await CurrentUser();
            var response = await _chatService.RemoveUserFromChatAsync(input.ChatId, input.UserId);
            return JsonResult(_mapper.Map<ChatResponse>(response));
        }

        [Authorize(Policy = Policies.User)]
        [HttpDelete("RemoveChat")]
        public async Task<IActionResult> RemoveChat(int chatId)
        { 
            var user = await CurrentUser();
            var chat = await _chatService.RemoveChatAsync(user.Id, chatId);
            return JsonResult(_mapper.Map<ChatResponse>(chat));
        }

        [HttpPost("EditChat")]
        public async Task<IActionResult> EditChat(UpdateChatInput input)
        {
            var user = await CurrentUser();
            var response = await _chatService.EditChatAsync(input.ChatId, input.ChatName);
            return JsonResult(_mapper.Map<ChatResponse>(response));
        }

        [HttpGet("GetChatDetails")]
        public async Task<IActionResult> GetChatDetails(int chatId)
        { 
            var user = await CurrentUser();
            var result = await _chatService.GetChatDetailsAsync(chatId);
            return JsonResult(result);
        }

        [HttpGet("GetChats")]
        public async Task<IActionResult> GetChats([FromQuery]SearchChatsInput input)
        {
            var mappedChat = _mapper.Map<List<ChatResponse>>(await _chatService.GetFiltredChat(input));
            return JsonResult(new PaginatedResponse<ChatResponse>(mappedChat.Count, mappedChat));
        }

        [HttpPost("CreateChat")]
        public async Task<IActionResult> CreateChat(InputChatCreate input)
        {
            var chatOwner = await CurrentUser();
            var chat = await _chatService.CreateChatAsync(input, chatOwner);
            return JsonResult(_mapper.Map<ChatResponse>(chat));
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
            var user = await CurrentUser();
            var messages = await _chatService.GetNewMessagesAsync(input.ChatId, user.Id, input.Skip, input.Take);
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
