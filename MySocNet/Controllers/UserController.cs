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

namespace MySocNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFriendService _friendService;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<User> _userRepository;
        private readonly IChatService _chatService;
      
        public UserController(
            IUserService userService,
            IFriendService friendService,
            IRepository<User> repository,
            IEmailSender emailSender,
            IChatService chatService
                             )
        {
            _userService = userService;
            _chatService = chatService;
            _friendService = friendService;
            _userRepository = repository;
            _emailSender = emailSender;
        }

        private async Task<User> GetUserByAccessToken()
        {
            var accessToken = await HttpContext.GetAccessToken();
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Authentication.AccessToken == accessToken);
            return user;
        }

        [HttpPost("GetSortedUserList")]
        public async Task<IActionResult> GetUsers(SearchUserInput userInput)
        {
            IQueryable<User> query = null;

            if (!string.IsNullOrWhiteSpace(userInput.Search))
            {
                query = _userRepository.GetWhereAsync(x =>
                    x.FirstName.ToUpper().Contains(userInput.Search) ||
                    x.SurName.ToUpper().Contains(userInput.Search));
            }

            query = _userService.GetSortedQuery(userInput, query);

            int totalCount = await query.CountAsync();

            var result = await query.Skip(userInput.Skip).Take(userInput.Take).ToListAsync();

            return Ok(new PaginatedOutput<User>(totalCount, result));
        }

        [HttpPost("SendToChat")]
        public async Task SendToChat(int reciveId, string message)
        {
            var user = await GetUserByAccessToken();
            await _chatService.SendMessage(user.Id, reciveId, message);
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
            var user = await GetUserByAccessToken();
            user.FirstName = input.FirstName;
            user.SurName = input.SurName;
            await _userRepository.UpdateAsync(user);
        }
        
        [HttpGet("GetUserById")]
        [Authorize(Policy = Policies.User)]
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        [HttpGet("GetFriendList")]
        [Authorize(Policy = Policies.User)]
        public async Task<IActionResult> GetFriendListAsync()
        {
            var user = await GetUserByAccessToken();
            var friendList = await _friendService.GetFriendListAsync(user.Id);
            return Ok(friendList);
        }

        [HttpPut("AddNewFriend")]
        [Authorize(Policy = Policies.User)]
        public async Task AddFriendToUserAsync(int userAddedId)
        {
            var user = await GetUserByAccessToken();
            await _friendService.AddFriendToUserAsync(user.Id, userAddedId);
        }

        [HttpPost("SendEmail")]
        [Authorize(Policy = Policies.User)]
        public async Task SendMail(string email, string subject, string message)
        {
            await _emailSender.SendEmailAsync(email, subject, message);
        }

        [HttpDelete("RemoveUser")]
        [Authorize(Policy = Policies.Admin)]
        public async Task RemoveUserByIdAsync(int id)
        {
           await _userRepository.RemoveAsyncById(id);
        }
    }
}