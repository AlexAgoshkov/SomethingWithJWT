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
using MySocNet.Services.Interfaces;
using Newtonsoft.Json;

namespace MySocNet.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly MyDbContext _myDbContext;

        public AuthenticationService(
            MyDbContext myDbContext, 
            IConfiguration config, 
            IUserService userService, 
            IRepository<User> userRepository,
            IMapper mapper)
        {
            _config = config;
            _userService = userService;
            _userRepository = userRepository;
            _myDbContext = myDbContext;
            _mapper = mapper;
        }

        public async Task CreateAuthTokenAsync(string userName)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.UserName == userName);

            DateTime created = DateTime.Now;
            DateTime expires = DateTime.Now.AddMinutes(1);

            string accessToken = await GenerateJWTTokenAsync(user, _config["Jwt:SecretKey"], expires);
            string refreshToken = await GenerateJWTTokenAsync(user, "Super_Pushka_Raketa_Turba_Boost", DateTime.Now.AddDays(30));

            if (!user.AuthenticationId.HasValue)
            {
                await CreateNewTokens(user, created, expires, accessToken, refreshToken);
            }
            else
            {
                await UpdateTokens(user, created, expires, accessToken, refreshToken);
            }
            _myDbContext.Users.Update(user);
            await _myDbContext.SaveChangesAsync();
        }

        public async Task<User> AuthenticateUserAsync(UserLogin loginCredentials)
        {
            var userByLogin = await _userRepository.GetWhereAsync(x => x.UserName == loginCredentials.UserName)
                .Include(x => x.Authentication).Include(x => x.ActiveKey).FirstOrDefaultAsync();

            User result = null;//TOdo

            if (HashService.Verify(loginCredentials.Password, userByLogin.Password))
            {
                result = _mapper.Map<User>(userByLogin);
            }

            return result;
        }

        private async Task CreateNewTokens(User user, DateTime created, DateTime expires, string accessToken, string refreshToken)
        {
            var jwtToken = new Authentication { Created = created, Expires = expires, AccessToken = accessToken, RefreshToken = refreshToken };
            await CreateTokenAsync(jwtToken);
            user.Authentication = jwtToken;
        }

        private async Task UpdateTokens(User user, DateTime created, DateTime expires, string accessToken, string refreshToken)
        {
            var auth = await _myDbContext.Authentications.FirstOrDefaultAsync(x => x.Id == user.AuthenticationId);
            auth.AccessToken = accessToken;
            auth.RefreshToken = refreshToken;
            auth.Created = created;
            auth.Expires = expires;
            _myDbContext.Authentications.Update(auth);
            user.AuthenticationId = auth.Id;
        }

        private async Task CreateTokenAsync(Authentication authentication)
        {
            await _myDbContext.Authentications.AddAsync(authentication);
            await _myDbContext.SaveChangesAsync();
        }

        private async Task<string> GenerateJWTTokenAsync(User user, string secretWord, DateTime expire)
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
    }
}
