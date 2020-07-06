﻿using System;
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
using MySocNet.Models.Email;
using MySocNet.OutPutData;
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
using MySocNet.OutputData;
using MySocNet.Enums;
using MySocNet.Extensions;

namespace MySocNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService,IEmailService emailService, IMapper mapper)
        {
            _userService = userService;
            _emailService = emailService;
            _mapper = mapper;
        }

        private async Task<User> GetUserByAccessToken()
        {
            var accessToken = await HttpContext.GetAccessToken();

            var user = await _userService.GetUsersAsync(x => x.Authentication.AccessToken == accessToken)
                .FirstOrDefaultAsync();
            return user;
        }

        [HttpPost("GetSortedUserList")]
        public async Task<IActionResult> GetUsers(SearchUserInput userInput)
        {
            IQueryable<User> query = null;

            if (!string.IsNullOrWhiteSpace(userInput.Search))
            {
                query = _userService.GetUsersAsync(x =>
                    x.FirstName.ToUpper().Contains(userInput.Search) ||
                    x.SurName.ToUpper().Contains(userInput.Search));
            }

            query = await _userService.GetSortedQuery(userInput, query);

            int totalCount = await query.CountAsync();

            var result = await query.Skip(userInput.Skip).Take(userInput.Take).ToListAsync();

            return Ok(new PaginatedOutput<User>(totalCount, result));
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAsync()
        {
            var users = await _userService.GetUsersAsync(x => true).Include(x => x.Friends).ToListAsync();
            return Ok(users); 
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
            await _userService.UpdateUserAsync(user);
        }
        
        [HttpGet("GetUserById")]
        [Authorize(Policy = Policies.User)]
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userService.GetUserByIdAsync(userId);
        }

        [HttpGet("GetFriendList")]
        [Authorize(Policy = Policies.User)]
        public async Task<IActionResult> GetFriendListAsync()
        {
            var user = await GetUserByAccessToken();

            var friendList = await _userService.GetFriendListAsync(user.Id);

            return Ok(friendList);
        }

        [HttpPut("AddNewFriend")]
        [Authorize(Policy = Policies.User)]
        public async Task AddFriendToUserAsync(int userAddedId)
        {
            var user = await GetUserByAccessToken();

            await _userService.AddFriendToUserAsync(user.Id, userAddedId);
        }

        [HttpPost("SendEmail")]
        [Authorize(Policy = Policies.User)]
        public async Task SendMail(string email, string subject, string message)
        {
            var sender = new Message(new string[] { email }, subject, message);
            await _emailService.SendEmail(sender);
        }

        [HttpDelete("RemoveUser")]
        [Authorize(Policy = Policies.Admin)]
        public async Task RemoveUserByIdAsync(int id)
        {
           await _userService.RemoveUserByIdAsync(id);
        }
    }
}