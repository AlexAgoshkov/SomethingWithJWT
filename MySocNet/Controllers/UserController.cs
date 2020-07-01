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
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Crypto.Engines;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection;

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

        private object GetProperty(string propertyName)
        {
            Type type = Type.GetType("MySocNet.Models.User", false, true);
            PropertyInfo property = type.GetProperty(propertyName);

            return property.Name;
        }

        [HttpGet("MYTEST")]
        public async Task<IActionResult> GetTestUsers(string name, int? skip, int? take, bool? isSort, string property)
        {
            var users = await _userService.GetUsersAsync();
            int totalCount = 0;
            List<User> list = null;

            if (skip.HasValue && take.HasValue)
            {
                list = users.Where(x => x.FirstName == name || x.SurName == name).Skip(skip.Value).Take(take.Value).ToList();
                totalCount = list.Count - take.Value;
            }
            else
            {
                list = users.Where(x => x.FirstName == name || x.SurName == name).ToList();
            }
            if (isSort.HasValue && isSort.Value)
            {
              // list
            }
            
            return Ok(new
            {
                list,
                totalCount,
            });
        }

        [HttpGet("GetAllUsers")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IEnumerable<User>> GetAsync()
        {
            return await _userService.GetUsersAsync();
        }

        [HttpGet]
        [Route("GetUserData")]
        [Authorize(Policy = Policies.User)]
        public IActionResult GetUserData()
        {
            return Ok("You are USER");
        }

        [HttpGet]
        [Route("GetAdminData")]
        [Authorize(Policy = Policies.Admin)]
        public IActionResult GetAdminData()
        {
            return Ok("You are ADMIN");
        }

        [HttpPost]
        [Route("UpdateUser")]
        [Authorize(Policy = Policies.User)]
        public async Task UpdateUserAsync(int id,UserUpdate input)
        {
            var user = await _userService.GetUserByIdAsync(id);
            user.FirstName = input.FirstName;
            user.SurName = input.SurName;
            user.Email = input.Email;
            
            await _userService.UpdateUserAsync(user);
        }
        
        [HttpGet("GetUserById")]
        [Authorize(Policy = Policies.User)]
        public async Task<User> GetUserByIdAsync(int userId)
        {
           
            return await _userService.GetUserByIdAsync(userId);
        }

        [HttpGet("GetUsersByName")]
        public async Task<IEnumerable<User>> GetUsersByName(string name, bool? isSort)
        {
            if (isSort.Value)
            {
                var users = await _userService.GetUsersBySurname(name);
                return users.OrderBy(x => x.FirstName);
            }

            return await _userService.GetUsersByName(name);
        }

        [HttpGet("GetUsersBySurName")]
        public async Task<IEnumerable<User>> GetUsersBySurname(string surname, bool? isSort)
        {
            if (isSort.Value)
            {
                var users = await _userService.GetUsersBySurname(surname);
                return users.OrderBy(x => x.SurName);
            }

            return await _userService.GetUsersBySurname(surname);
        }

        [HttpGet("GetFriendList")]
        [Authorize(Policy = Policies.User)]
        public async Task<IEnumerable<UserOutPut>> GetFriendListAsync(int userId)
        {
            //Request.Headers["key"]
            return await _userService.GetFriendListAsync(userId);
        }

        [HttpPut("AddNewFriend")]
        [Authorize(Policy = Policies.User)]
        public async Task AddFriendToUserAsync(int userId, int userAddedId)
        {
            await _userService.AddFriendToUserAsync(userId, userAddedId);
        }

        [HttpPost("SendEmail")]
        [Authorize(Policy = Policies.User)]
        public async Task Get(string email, string subject, string message)
        {
            var sender = new Message(new string[] { email }, subject, message);
            await _emailService.SendEmail(sender);

            await _userService.GetUsersAsync();
        }

        [HttpDelete("RemoveUser")]
        [Authorize(Policy = Policies.Admin)]
        public async Task RemoveUserByIdAsync(int id)
        {
           await _userService.RemoveUserByIdAsync(id);
        }
    }
}
    