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
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using Microsoft.AspNetCore.Authentication;

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

        [HttpPost("GetUserList")]
        public async Task<IActionResult> GetUsers(SearchUserInput userInput)
        {
            var custumerOb = new User();
            var isAllPropertiesNull = custumerOb.GetType().GetProperty(userInput.OrderKey).GetValue(custumerOb);
                                               
                                                
            List<User> result = null;

            if (string.IsNullOrWhiteSpace(userInput.Name))
            {
                result = await _userService.GetUsersAsync(x => true).ToListAsync();
            }
            else
            {
                

                if (userInput.IsSort)
                {
                    result = await _userService.GetUsersAsync(x => x.FirstName == userInput.Name || x.SurName == userInput.Name)
                    .OrderBy(x => isAllPropertiesNull)
                    .ToListAsync();
                    //result = await _userService.GetUsersAsync(x => x.FirstName == userInput.Name || x.SurName == userInput.Name)
                    //    .ToListAsync();
                    //result = result.OrderBy(x => typeof(User).GetProperty(userInput.OrderKey).GetValue(x)).ToList();
                }
                else
                {
                    //result = await _userService.GetUsersAsync(x => x.FirstName == userInput.Name || x.SurName == userInput.Name)
                    //    .ToListAsync();
                    //result = result.OrderByDescending(x => typeof(User).GetProperty(userInput.OrderKey).GetValue(x)).ToList();
                }
            }

            result = result.Skip(userInput.Skip).Take(userInput.Take).ToList();
          
            int totalCount = result.Count;

            return Ok(new
            {
                result,
                totalCount
            });
        }

        [HttpGet("GetAllUsers")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> GetAsync()
        {
            var users = await _userService.GetUsersAsync(x => true).ToListAsync();
            return Ok(new { users }); 
        }

        [HttpGet]
        [Route("GetUserData")]
        [Authorize(Policy = Policies.User)]
        public async Task<IActionResult> GetUserData()
        {
            var accessToken = await HttpContext.GetAccessTokenOne();

            var name = _userService.GetUsersAsync(x => x.Authentication.AccessToken == accessToken);

            return Ok(new { accessToken, name});
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

            await _userService.GetUsersAsync(x => true).ToListAsync();
        }

        [HttpDelete("RemoveUser")]
        [Authorize(Policy = Policies.Admin)]
        public async Task RemoveUserByIdAsync(int id)
        {
           await _userService.RemoveUserByIdAsync(id);
        }
    }
}

public static class HttpContextExtension 
{
    public async static Task<string> GetAccessTokenOne(this HttpContext httpContext)
    {
        return await httpContext.GetTokenAsync("access_token");
    }
} 