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
       
        public ChatController(
            IRepository<User> userRepository,
            IChatService chatService,
            IMyLogger myLogger,
            IMapper mapper
            ) : base(userRepository)
        {
            _chatService = chatService;
            _mapper = mapper;
        }

        [HttpPost("ForwardMessage")]
        public async Task<IActionResult> ForwardMessage(ForwardMessageInput input)
        {
            var currentUser = await CurrentUser();
            var message = await _chatService.ForwardMessageAsync(currentUser, input.MessageId, input.ChatId);
            return JsonResult(message);
        }

        [HttpPost("InviteUserToChat")]
        public async Task<IActionResult> InviteUserToChat(UserToChatInput input)
        {
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
            var response = await _chatService.EditChatAsync(input.ChatId, input.ChatName);
            return JsonResult(_mapper.Map<ChatResponse>(response));
        }

        [HttpGet("GetChatDetails")]
        public async Task<IActionResult> GetChatDetails(int chatId)
        { 
            var result = await _chatService.GetChatDetailsAsync(chatId);
            return JsonResult(result);
        }

        [HttpGet("SearchChats")]
        public async Task<IActionResult> SearchChats([FromQuery]SearchChatsInput input)
        {
            var chats = await _chatService.GetFiltredChatAsync(input);
            var chatResponse = _mapper.Map<PaginatedResponse<ChatResponse>>(chats);
            return JsonResult(chatResponse);
        }

        [HttpGet("GetUserChats")]
        public async Task<IActionResult> GetUserChats([FromQuery]UserChatsInput input)
        {
            var currentUser = await CurrentUser();
            return JsonResult(await _chatService.GetUserChatsAsync(currentUser, input));
        }

        [HttpPost("AddToHiddenList")]
        public async Task<IActionResult> AddToHiddenList(int chatId)
        {
            var currentUser = await CurrentUser();
            return JsonResult(await _chatService.AddToHiddenListAsync(currentUser, chatId));
        }

        [HttpPost("CreateChat")]
        public async Task<IActionResult> CreateChat(CreateChatInput input)
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
        public async Task<IActionResult> SendMessage([FromQuery]SendMessageInput input)
        {
            var messageSender = await CurrentUser();
            var message = await _chatService.SendMessageAsync(messageSender, input);
            return JsonResult(message);
        }
    }
}