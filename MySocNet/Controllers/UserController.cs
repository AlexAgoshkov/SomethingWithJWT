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
using Microsoft.Extensions.Logging;
using MySocNet.Logger;
using MySocNet.Exceptions;
using DapperSqlite.Services;
using Microsoft.AspNetCore.Http.Features;

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
        private readonly IHttpContextAccessor _accessor;
        private readonly IMyLogger _logger;
        private readonly ILog _log;
        
        public UserController(
            IUserService userService,
            IFriendService friendService,
            IRepository<User> userRepository,
            IRepository<UserChat> userChatRepository,
            IRepository<Chat> chatRepository,
            IRepository<Message> messageRepository,
            IHttpContextAccessor accessor,
            IEmailService emailSender,
            IMyLogger logger,
            ILog log) : base(userRepository)
        {
            _userService = userService;
            _friendService = friendService;
            _userRepository = userRepository;
            _userChatRepository = userChatRepository;
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
            _accessor = accessor;
            _emailSender = emailSender;
            _log = log;
            _logger = logger;
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

        [HttpGet("AdminDashBoard")]
        [Authorize(Roles = Policies.Admin)]
        public IActionResult Dashboard()
        {
            var activedUser = new StatisticsActivedUserService();
            var newUser = new StatisticsNewUserService();
            var sentMessage = new StatisticsSentMessageService();

            return JsonResult(new 
            { 
                ActivedUser = activedUser.GetActivedUsersCount(),
                NewUser = newUser.GetNewUsersCount(), 
                SentMessages = sentMessage.GetNewMessageCount() 
            });
        }

        [HttpGet]
        [Route("GetCurrentUser(TEST)")]
        [Authorize]
        public async Task<IActionResult> GetCurrerUser()
        {
            return JsonResult(await CurrentUser());
        }

        [HttpGet]
        [Route("GetUserData")]
        [Authorize(Roles = Policies.Admin + "," + Policies.User)]
        public async Task<IActionResult> GetUserData()
        {
            var user = await CurrentUser();
            var remoteIpAddress = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress;
            var ip = HttpContext.Connection.RemoteIpAddress;
           // var ip = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            // await _logger.AddLog(new LogData { Category = "Alert", Message = "User Got Data about himhelf", UserId = user.Id, User = user.UserName });
            return JsonResult(user);
        }

        [HttpGet]
        [Route("GetUserDataS")]
        [Authorize(Roles = Policies.User)]
        public IActionResult Index()
        {        
            var userAgent = Request.Headers["User-Agent"];
            var result = DeviceDetectorNET.DeviceDetector.GetInfoFromUserAgent(userAgent);

            return JsonResult(new
            {
                result.Match.DeviceType,
                result.Match.OsFamily,
                result.Match.BrowserFamily
            });
        }

        [HttpGet]
        [Route("GetAdminData")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> GetAdminData()
        {
            var user = await CurrentUser();
            _log.Information($"User Id {user.Id} Login {user.UserName} checked information about himself");
            return JsonResult(user);
        }

        [HttpPost]
        [Route("UpdateUser")]
        [Authorize(Roles = Policies.Admin + "," + Policies.User)]
        public async Task UpdateUserAsync(UserUpdate input)
        {
            var user = await CurrentUser();
            user.FirstName = input.FirstName;
            user.SurName = input.SurName;
            _log.Information($"User Id {user.Id} Login {user.UserName} Added First Name {user.FirstName} and Sur Name {user.SurName}");
            await _userRepository.UpdateAsync(user);
        }
        
        [HttpGet("GetUserById")]
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId) ??
            throw new EntityNotFoundException("User not found");
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
        public async Task<IActionResult> RemoveUserByIdAsync(int id)
        {
            var user = await _userRepository.GetWhere(x => x.Id == id).FirstOrDefaultAsync();
            if (user == null)
                return NotFound("User not found");

            var userChats = await _userChatRepository.GetWhere(x => x.UserId == id).ToListAsync();
                
            if (userChats.Any())
            {
                var chat = _chatRepository.GetWhere(x => x.ChatOwnerId == id);
                var usersChats = _userChatRepository.GetWhere(x => chat.Select(x => x.ChatOwnerId).Contains(x.ChatId));
                var messages = _messageRepository.GetWhere(x => chat.Select(x => x.Id).Contains(x.ChatId));
                await _messageRepository.RemoveRangeAsync(messages);
                await _userChatRepository.RemoveRangeAsync(userChats);
                await _chatRepository.RemoveRangeAsync(chat);   
            }
                
            await _userRepository.RemoveAsync(user);
            var admin = await CurrentUser();
            return JsonResult(user);
        }
    }
}