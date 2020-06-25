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
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;
        public LoginController(IConfiguration config, IUserService userService,IAuthenticationService authenticationService, IMapper mapper)
        {
            _config = config;
            _userService = userService;
            _authenticationService = authenticationService;
            _mapper = mapper;
        }

        [HttpPost("RefreshToken")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> UpdateAccess(string refreshToken)
        {
            var user = await _userService.GetUserByRefreshTokenAsync(refreshToken);
            if (user != null)
            {
                var newToken = await _authenticationService.GenerateJWTToken(user, _config["Jwt:SecretKey"], DateTime.Now.AddMinutes(2));
                var newRefreshToken = await _authenticationService.GenerateJWTToken(user, "Super_Pushka_Raketa_Turba_Boost", DateTime.Now.AddDays(30));
                await _authenticationService.CreateTokenPairAsync(user, DateTime.Now, DateTime.Now.AddMinutes(2), newToken, newRefreshToken);

                return Ok(new { newToken, newRefreshToken });
            }
            return BadRequest();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLogin login)
        {
            var user = await _authenticationService.AuthenticateUser(login);

            if (user == null)
            {
                return BadRequest();
            }

            var tokenString = await _authenticationService.GenerateJWTToken(user, _config["Jwt:SecretKey"], DateTime.Now.AddMinutes(2));
            var tokenRefresh = await _authenticationService.GenerateJWTToken(user, "Super_Pushka_Raketa_Turba_Boost", DateTime.Now.AddDays(30));
            await _authenticationService.CreateTokenPairAsync(user, DateTime.Now, DateTime.Now.AddMinutes(2), tokenString, tokenRefresh);

            return Ok(new
            {
                token = tokenString,
                tokenRefresh = tokenRefresh,
                userDetails = user,
            });
        }


        [HttpPost("Registration")]
        public async Task RegistrationAsync(UserInput input)
        {
            var user = _mapper.Map<User>(input);

            await _userService.CreateUserAsync(user);
        }

        //private void setTokenCookie(string token)
        //{
        //    var cookieOptions = new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Expires = DateTime.UtcNow.AddDays(7)
        //    };
        //    Response.Cookies.Append("refreshToken", token, cookieOptions);
        //}
    }
}