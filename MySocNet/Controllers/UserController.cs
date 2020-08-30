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
using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace MySocNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ApiControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly IFriendService _friendService;
        private readonly IEmailService _emailSender;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserChat> _userChatRepository;
        private readonly IRepository<Chat> _chatRepository;
        private readonly IRepository<Message> _messageRepository;
        private readonly ILog _log;
        
        public UserController(
            IConfiguration config,
            IUserService userService,
            IFriendService friendService,
            IRepository<User> userRepository,
            IRepository<UserChat> userChatRepository,
            IRepository<Chat> chatRepository,
            IRepository<Message> messageRepository,
            IEmailService emailSender,
            IMyLogger logger,
            ILog log) : base(userRepository)
        {
            _config = config;
            _userService = userService;
            _friendService = friendService;
            _userRepository = userRepository;
            _userChatRepository = userChatRepository;
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
            _emailSender = emailSender;
            _log = log;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return JsonResult(await _userRepository.GetAllAsync());
        }

        [HttpPost("GetSortedUserList")]
        public async Task<IActionResult> GetUsers(SearchUserInput userInput)
        {
            return JsonResult(await _userService.GetPaginatedUsers(userInput));
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
        [Route("GetUserIp")]
        public async Task<IActionResult> GetUserIp()
        {
            string url5 = "https://accounts.spotify.com/api/token";
            var clientid = _config["Spotify:ClientId"];
            var clientsecret = _config["Spotify:ClientSecret"];

            var encode_clientid_clientsecret = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", clientid, clientsecret)));

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url5);

            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Accept = "application/json";
            webRequest.Headers.Add("Authorization: Basic " + encode_clientid_clientsecret);

            var request = ("grant_type=client_credentials");
            byte[] req_bytes = Encoding.ASCII.GetBytes(request);
            webRequest.ContentLength = req_bytes.Length;

            Stream strm = webRequest.GetRequestStream();
            strm.Write(req_bytes, 0, req_bytes.Length);
            strm.Close();

            HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
            string json = "";
            using (Stream respStr = resp.GetResponseStream())
            {
                using (StreamReader rdr = new StreamReader(respStr, Encoding.UTF8))
                {
                    json = rdr.ReadToEnd();
                    rdr.Close();
                }
            }
            return JsonResult(json);
        }

        [HttpGet]
        [Route("GetUserData")]
        [Authorize(Roles = Policies.Admin + "," + Policies.User)]
        public async Task<IActionResult> GetUserData()
        {
            var user = await CurrentUser();
            return JsonResult(user);
        }

        [HttpGet]
        [Route("GetUserTest")]
        [Authorize(Roles = Policies.User)]
        public IActionResult GetUserTest()
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
         
        [HttpGet]
        [Route("ChangeUserRole")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> SetRole(int userId, string role)
        {
            return JsonResult(await _userService.ChangeRole(userId, role));
        }

        [HttpPost]
        [Route("UpdateUser")]
        [Authorize(Roles = Policies.Admin + "," + Policies.User)]
        public async Task UpdateUserAsync(UserUpdate input)
        {
            var user = await CurrentUser();
            user.FirstName = input.FirstName;
            user.SurName = input.SurName;
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