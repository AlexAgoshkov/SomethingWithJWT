using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
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
using MySocNet.InputData;
using MySocNet.Models;
using MySocNet.Services;
using Newtonsoft.Json;

namespace MySocNet.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly MyDbContext _myDbContext;

        public AuthenticationService(MyDbContext myDbContext, IConfiguration config, IUserService userService, IMapper mapper)
        {
            _config = config;
            _userService = userService;
            _myDbContext = myDbContext;
            _mapper = mapper;
        }

        public async Task<User> AuthenticateUserAsync(UserLogin loginCredentials)
        {
            var userByLogin = await _userService.GetUserByUserNameAsync(loginCredentials.UserName);
            User result = null;

            if (HashService.Verify(loginCredentials.Password, userByLogin.Password))
            {
                result = _mapper.Map<User>(userByLogin);
            }

            return result;
        }

        public async Task CreateTokenAsync(Authentication authentication)
        {
            await _myDbContext.Authentications.AddAsync(authentication);
            await _myDbContext.SaveChangesAsync();
        }

        public async Task CreateTokenPairAsync(User userInfo, DateTime created, DateTime expires, string accessToken, string refreshToken)
        {
            if (!userInfo.AuthenticationId.HasValue)
            {
                var jwtToken = new Authentication { Created = created, Expires = expires, AccessToken = accessToken, RefreshToken = refreshToken };
                await CreateTokenAsync(jwtToken);
                userInfo.Authentication = jwtToken;
            }
            else
            {
                var auth = await _myDbContext.Authentications.FirstOrDefaultAsync(x => x.AuthenticationId == userInfo.AuthenticationId);
                auth.AccessToken = accessToken;
                auth.RefreshToken = refreshToken;
                auth.Created = created;
                auth.Expires = expires;
                _myDbContext.Authentications.Update(auth);
                userInfo.AuthenticationId = auth.AuthenticationId;
            }
            _myDbContext.Users.Update(userInfo);
            await _myDbContext.SaveChangesAsync();
        }

        public async Task CreateActiveKeyAsync(string key)
        {
            var activeKey = new ActiveKey { Key = key, Created = DateTime.Now, IsActive = false };
            _myDbContext.ActiveKeys.Add(activeKey);
            await _myDbContext.SaveChangesAsync();
        }
        public async Task<ActiveKey> GetActiveKeyByNameAsync(string key)
        {
            return await _myDbContext.ActiveKeys.FirstOrDefaultAsync(x => x.Key == key);
        }

        public async Task AddActiveKeyToUserAsync(int userId, int keyId)
        {
            var user = await _myDbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            user.ActiveKeyId = keyId;
            _myDbContext.Users.Update(user);
            await _myDbContext.SaveChangesAsync();
        }

        public async Task<string> GenerateJWTTokenAsync(User user, string secretWord, DateTime expire)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretWord));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("role", user.UserRole),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expire,
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); 
            return path.Substring(0, 8); 
        }
    }
}
