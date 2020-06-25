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
using MySocNet.Models.Email;
using MySocNet.OutPutData;
using MySocNet.Services;

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

        [HttpGet("GetAllUsers")]
        public async Task<IEnumerable<User>> GetAsync()
        {
            return await _userService.GetUsersAsync();
        }

        [HttpGet]
        [Route("GetUserData")]
        [Authorize(Policy = "User")]
        public IActionResult GetUserData()
        {
            return Ok("You are USER");
        }

        [HttpGet]
        [Route("GetAdminData")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetAdminData()
        {
            return Ok("You are ADMIN");
        }

        [HttpGet("GetUserById")]
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userService.GetUserByIdAsync(userId);
        }

        [HttpGet("GetFriendList")]
        public async Task<IEnumerable<UserOutPut>> GetFriendListAsync(int userId)
        {
            return await _userService.GetFriendListAsync(userId);
        }

        [HttpPut("AddNewFriend")]
        public async Task AddFriendToUserAsync(int userId, int userAddedId)
        {
            await _userService.AddFriendToUserAsync(userId, userAddedId);
        }

        [HttpPost("SendEmail")]
        public async Task Get(string email, string subject, string message)
        {
            var sender = new Message(new string[] { email }, subject, message);
            await _emailService.SendEmail(sender);

            await _userService.GetUsersAsync();
        }

    }
}
