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
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MySocNet.InputData;
using MySocNet.Models;
using MySocNet.Services;
using Newtonsoft.Json;

namespace MySocNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;

        public LoginController(IConfiguration config, IUserService userService,IEmailSender emailSender, IAuthenticationService authenticationService, IMapper mapper)
        {
            _config = config;
            _userService = userService;
            _emailSender = emailSender;
            _authenticationService = authenticationService;
            _mapper = mapper;
        }

        [HttpPost("RefreshToken")]
        [Authorize(Policy = Policies.User)]
        public async Task<IActionResult> UpdateAccess(string refreshToken)
        {
            var user = await _userService.GetUserByRefreshTokenAsync(refreshToken);
            if (user != null)
            {
                var newToken = await _authenticationService.GenerateJWTTokenAsync(user, _config["Jwt:SecretKey"], DateTime.Now.AddMinutes(2));
                var newRefreshToken = await _authenticationService.GenerateJWTTokenAsync(user, "Super_Pushka_Raketa_Turba_Boost", DateTime.Now.AddDays(30));
                await _authenticationService.CreateTokenPairAsync(user, DateTime.Now, DateTime.Now.AddMinutes(2), newToken, newRefreshToken);

                return Ok(new { newToken, newRefreshToken });
            }
            return BadRequest();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLogin login)
        {
            var user = await _authenticationService.AuthenticateUserAsync(login);

            if (user == null || user.ActiveKey == null || !user.ActiveKey.IsActive)
            {
                return BadRequest();
            }

            var tokenString = await _authenticationService.GenerateJWTTokenAsync(user, _config["Jwt:SecretKey"], DateTime.Now.AddMinutes(30));
            var tokenRefresh = await _authenticationService.GenerateJWTTokenAsync(user, "Super_Pushka_Raketa_Turba_Boost", DateTime.Now.AddDays(30));
            await _authenticationService.CreateTokenPairAsync(user, DateTime.Now, DateTime.Now.AddMinutes(2), tokenString, tokenRefresh);

            return Ok(new
            {
                tokenString,
                tokenRefresh,
                user,
            });
        }

        [HttpPost("Registration")]
        [AllowAnonymous]
        public async Task RegistrationAsync(UserRegistration input)
        {
            var user = _mapper.Map<User>(input);
            var key = _authenticationService.GetRandomString();
        
            await _authenticationService.CreateActiveKeyAsync(key);
            var userKey = await _authenticationService.GetActiveKeyByNameAsync(key);
           
            user.ActiveKeyId = userKey.Id;
            await _userService.CreateUserAsync(user);

            var confirmationLink = Url.Action(nameof(ConfirmEmail),
                                 "Login",
                                 new { user.ActiveKey.Key },
                                 Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Confirmation email link", confirmationLink);
        }

        [HttpGet("ConfirmEmail")]
        public async Task ConfirmEmail(string Key)
        {
            await _userService.ConfirmEmailAsync(Key);
        }
    }
}