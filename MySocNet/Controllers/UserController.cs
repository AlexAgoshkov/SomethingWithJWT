using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MySocNet.InputData;
using MySocNet.Models;
using MySocNet.Response;
using MySocNet.Services;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Crypto.Engines;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using Microsoft.AspNetCore.Authentication;
using System.Data;
using System.Globalization;
using Microsoft.VisualBasic;
using MySocNet.Response;
using MySocNet.Enums;
using MySocNet.Extensions;
using MySocNet.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR;
using MySocNet.Hubs;
using Microsoft.AspNetCore.Razor.Language;
using Newtonsoft.Json;
using MySocNet.Input;
using System.IO;
using System.ComponentModel;

namespace MySocNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFriendService _friendService;
        private readonly IEmailService _emailSender;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserChat> _userChatRepository;
        private readonly IRepository<Chat> _chatRepository;
        private readonly IRepository<Message> _messageRepository;
        private readonly IImageService _imageService;
        private readonly ILog _log;
        
        public UserController(
            IUserService userService,
            IFriendService friendService,
            IRepository<User> userRepository,
            IRepository<UserChat> userChatRepository,
            IRepository<Chat> chatRepository,
            IRepository<Message> messageRepository,
            IImageService imageService,
            IEmailService emailSender,
            ILog log) : base(userRepository)
        {
            _userService = userService;
            _friendService = friendService;
            _imageService = imageService;
            _userRepository = userRepository;
            _userChatRepository = userChatRepository;
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
            _emailSender = emailSender;
            _log = log;
        }

        private async Task<User> GetUserByAccessToken()
        {
            var accessToken = await HttpContext.GetAccessToken();
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Authentication.AccessToken == accessToken)
                ?? throw new UnauthorizedAccessException();
            return user;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return JsonResult(await _userRepository.GetAllAsync());
        }

        [HttpPost("GetSortedUserList")]
        public async Task<IActionResult> GetUsers(SearchUserInput userInput)
        {
            IQueryable<User> query = null;

            if (!string.IsNullOrWhiteSpace(userInput.Search))
            {
                query = _userRepository.GetWhere(x =>
                    x.FirstName.ToUpper().Contains(userInput.Search) ||
                    x.SurName.ToUpper().Contains(userInput.Search));
            }

            query = _userService.GetSortedQuery(userInput, query);

            int totalCount = await query.CountAsync();

            var result = await query.Skip(userInput.Skip).Take(userInput.Take).ToListAsync();

            return Ok(new PaginatedResponse<User>(totalCount, result));
        }

        [HttpGet]
        [Route("GetUserData")]
        [Authorize(Policy = Policies.User)]
        public async Task<IActionResult> GetUserData()
        {
            User user = await GetUserByAccessToken();
            return Ok(user);
        }

        [HttpGet]
        [Route("GetAdminData")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> GetAdminData()
        {
            User user = await GetUserByAccessToken();
            return Ok(user);
        }

        [HttpPost]
        [Route("UpdateUser")]
        [Authorize(Policy = Policies.User)]
        public async Task UpdateUserAsync(UserUpdate input)
        {
            var user = await CurrentUser();
            user.FirstName = input.FirstName;
            user.SurName = input.SurName;
            await _userRepository.UpdateAsync(user);
        }
        
        [HttpGet("GetUserById")]
        [Authorize(Policy = Policies.User)]
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId) ??
            throw new ArgumentException("User not found");
        }

        [HttpGet("GetFriendList")]
        [Authorize(Policy = Policies.User)]
        public async Task<IActionResult> GetFriendListAsync()
        {
            var user = await CurrentUser();
            var friendList = await _friendService.GetFriendListAsync(user.Id);
            return Ok(friendList);
        }

        [HttpPut("AddNewFriend")]
        [Authorize(Policy = Policies.User)]
        public async Task AddFriendToUserAsync(int userAddedId)
        {
            var user = await CurrentUser();
            await _friendService.AddFriendToUserAsync(user.Id, userAddedId);
        }

        [HttpPost("SendEmail")]
        [Authorize(Policy = Policies.User)]
        public async Task SendMail(SendMailInput input)
        {
            await _emailSender.SendEmailAsync(input.To, input.Subject, input.Message);
        }

        [HttpDelete("RemoveUser")]
       
        public async Task<IActionResult> RemoveUserByIdAsync(int id)//--------------- CAN NOT REMOVE USER!!!
        {
            try
            {
                var userChats = _userChatRepository.GetWhere(x => x.UserId == id);
                await _userChatRepository.RemoveRangeAsync(userChats);

                var chat = await _chatRepository.GetWhere(x => x.ChatOwnerId == id).ToListAsync();
                if (chat != null)
                {
                    var usersChats = _userChatRepository.GetWhere(x => chat.Select(x => x.ChatOwnerId).Contains(x.ChatId));
                    var messages = _messageRepository.GetWhere(x => chat.Select(x => x.Id).Contains(x.ChatId));
                    await _messageRepository.RemoveRangeAsync(messages);
                    await _userChatRepository.RemoveRangeAsync(userChats);
                    await _chatRepository.RemoveRangeAsync(chat);   
                }
                
                var user = _userRepository.GetWhere(x => x.Id == id);
                await _userRepository.RemoveAsync(await user.FirstOrDefaultAsync());
                return JsonResult(user);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}