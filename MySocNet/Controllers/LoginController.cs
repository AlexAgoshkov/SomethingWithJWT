using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MySocNet.Input;
using MySocNet.InputData;
using MySocNet.Models;
using MySocNet.Response;
using MySocNet.Services;
using MySocNet.Services.Interfaces;
using Newtonsoft.Json;
using NLog.Fluent;
using DapperSqlite.Models;
using DapperSqlite.Services;
using DeviceDetectorNET;
using MySocNet.Exceptions;

namespace MySocNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ApiControllerBase
    {
        private readonly IUserService _userService;   
        private readonly IEmailService _emailSender;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAccountActivationService _accountActiveService;
        private readonly ILogger<LoginController> _logger;
        private readonly IMapper _mapper;

        public LoginController(
            IConfiguration config, 
            IUserService userService,
            IEmailService emailSender, 
            IAuthenticationService authenticationService,
            IAccountActivationService accountActiveService,
            IRepository<User> userRepository,
            ILogger<LoginController> logger,
            IMapper mapper) : base(userRepository)
        {
            _userService = userService;
            _emailSender = emailSender;
            _authenticationService = authenticationService;
            _accountActiveService = accountActiveService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> UpdateAccess(UpdateAccessTokenInput input)
        {
            var user = await _userService.GetUserByRefreshTokenAsync(input.RefreshToken);
            var token = await _authenticationService.CreateAuthTokenAsync(user.UserName);   
            return JsonResult(token);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLogin login)
        {
            var detect = await GetUserHardwareInfo();

            var user = await _authenticationService.AuthenticateUserAsync(login);

            await _authenticationService.DetectUserAsync(detect, user, GetConfirmIdentity(user));

            var token = await _authenticationService.CreateAuthTokenAsync(login.UserName);
            
            return JsonResult(token);
        }

        [HttpPost("Registration")]
        [AllowAnonymous]
        public async Task<IActionResult> RegistrationAsync(UserRegistration input)
        {
            var detect = await GetUserHardwareInfo();
            var user = await _accountActiveService.UserRegistration(input, detect);

            await SendConfirmEmail(user);

            StatisticsNewUserService statisticsNewUser = new StatisticsNewUserService();
            statisticsNewUser.AddNewUser(input.UserName);

            return JsonResult(_mapper.Map<UserResponse>(user));
        }

        [HttpGet("ConfirmEmail")]
        public async Task ConfirmEmail(string key)
        {
            await _accountActiveService.ConfirmEmailAsync(key);
            StatisticsActivedUserService statisticsActived = new StatisticsActivedUserService();
            statisticsActived.AddActivedUser(key);
        }

        [HttpGet("ConfirmIdentity")]
        public async Task ConfirmIdentity(string key)
        {
            await _accountActiveService.ConfirmEmailAsync(key);
        }

        private async Task SendConfirmEmail(User user)
        {
            string confirmationLink = GetConfirmLink(user);

            await _emailSender.SendEmailAsync(user.Email, "Confirmation email link", confirmationLink);
        }
    }
}