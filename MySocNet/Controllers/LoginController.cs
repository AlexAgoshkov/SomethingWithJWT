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
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly IRepository<User> _userRepository;
        private readonly IEmailService _emailSender;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAccountActivationService _accountActiveService;
        private readonly IRepository<Detect> _detectRepository; 
        private readonly ILogger<LoginController> _logger;
        private readonly IMapper _mapper;

        public LoginController(
            IConfiguration config, 
            IUserService userService,
            IEmailService emailSender, 
            IAuthenticationService authenticationService,
            IAccountActivationService accountActiveService,
            IRepository<User> userRepository,
            IRepository<Detect> detectRepository,
            ILogger<LoginController> logger,
            IMapper mapper) : base(userRepository)
        {
            _config = config;
            _userService = userService;
            _userRepository = userRepository;
            _emailSender = emailSender;
            _authenticationService = authenticationService;
            _accountActiveService = accountActiveService;
            _detectRepository = detectRepository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> UpdateAccess(UpdateAccessTokenInput input)
        {
            var response = new RefreshTokenResponse();
            var user = await _userService.GetUserByRefreshTokenAsync(input.RefreshToken);
            await _authenticationService.CreateAuthTokenAsync(user.UserName);
            response.AccessToken = user.Authentication.AccessToken;
            response.RefreshToken = user.Authentication.RefreshToken;
            return JsonResult(response);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLogin login)
        {
            var response = new LoginResponse();

            var detect = await GetUserHardwareInfo();

            response.User = await _authenticationService.AuthenticateUserAsync(login, detect, HttpContext);
            await _authenticationService.CreateAuthTokenAsync(login.UserName);
            response.AccessToken = response.User.Authentication.AccessToken;
            response.RefreshToken = response.User.Authentication.RefreshToken;

            return JsonResult(response);
        }

        [HttpPost("Registration")]
        [AllowAnonymous]
        public async Task<IActionResult> RegistrationAsync(UserRegistration input)
        {
            var detect = await GetUserHardwareInfo();
            var user = await _accountActiveService.UserRegistration(input, detect);

            await SendConfirmEmail(user);

            _logger.LogInformation($"User Id: {user.Id} Got Confirmation Link to his/her Email {user.Email}");

            StatisticsNewUserService statisticsNewUser = new StatisticsNewUserService();
            statisticsNewUser.AddNewUser(input.UserName);

            return JsonResult(_mapper.Map<UserResponse>(user));
        }

        [HttpGet("ConfirmEmail")]
        public async Task ConfirmEmail(string key)
        {
            await _accountActiveService.ConfirmEmailAsync(key);
            _logger.LogInformation($"Email with {key} was Confirmed");
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
            var confirmationLink = Url.ActionLink(nameof(ConfirmEmail),
                                    "Login",
                                    new { user.ActiveKey.Key },
                                    Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Confirmation email link", confirmationLink);
        }
    }
}