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
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MySocNet.Input;
using MySocNet.InputData;
using MySocNet.Models;
using MySocNet.Response;
using MySocNet.Services;
using MySocNet.Services.Interfaces;
using Newtonsoft.Json;

namespace MySocNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly IRepository<User> _userRepository;
        private readonly IEmailSender _emailSender;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAccountActivationService _accountActiveService;
        private readonly IMapper _mapper;

        public LoginController(
            IConfiguration config, 
            IUserService userService,
            IEmailSender emailSender, 
            IAuthenticationService authenticationService,
            IAccountActivationService accountActiveService,
            IRepository<User> userRepository,
            IMapper mapper)
        {
            _config = config;
            _userService = userService;
            _userRepository = userRepository;
            _emailSender = emailSender;
            _authenticationService = authenticationService;
            _accountActiveService = accountActiveService;
            _mapper = mapper;
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> UpdateAccess(UpdateAccessTokenInput input)
        {
            var response = new RefreshTokenResponse();
            var user = await _userService.GetUserByRefreshTokenAsync(input.RefreshToken);
            if (user != null)
            {
                await _authenticationService.CreateAuthTokenAsync(user.UserName);
                response.AccessToken = user.Authentication.AccessToken;
                response.RefreshToken = user.Authentication.RefreshToken;
                return Ok(response);
            }
            return BadRequest();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLogin login)
        {
            var response = new LoginResponse();

            response.User = await _authenticationService.AuthenticateUserAsync(login);

            if (response.User == null || response.User.ActiveKey == null || !response.User.ActiveKey.IsActive)
            {
                return BadRequest();
            }

            await _authenticationService.CreateAuthTokenAsync(login.UserName);

            response.AccessToken = response.User.Authentication.AccessToken;
            response.RefreshToken = response.User.Authentication.RefreshToken;

            return Ok(response);
        }

        [HttpPost("Registration")]
        [AllowAnonymous]
        public async Task RegistrationAsync(UserRegistration input)
        {
            await _accountActiveService.CreateActiveKeyAsync(input);

            var user = await _userRepository.GetWhere(x => x.UserName == input.UserName)
                .Include(x => x.ActiveKey).FirstOrDefaultAsync();

            var confirmationLink = Url.Action(nameof(ConfirmEmail),
                                 "Login",
                                 new { user.ActiveKey.Key },
                                 Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Confirmation email link", confirmationLink);
        }

        [HttpGet("ConfirmEmail")]
        public async Task ConfirmEmail(string Key)
        {
            await _accountActiveService.ConfirmEmailAsync(Key);
        }
    }
}