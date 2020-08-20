﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MySocNet.Exceptions;
using MySocNet.Input;
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
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Detect> _detectRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private const int TokenMinutes = 360;
        private const string LocalAddress = "http://localhost:";


        public AuthenticationService(
            IConfiguration config, 
            IRepository<User> userRepository,
            IRepository<Detect> detectRepository,
            IEmailService emailService,
            IMapper mapper)
        {
            _config = config;
            _userRepository = userRepository;
            _detectRepository = detectRepository;
            _emailService = emailService;
            _mapper = mapper;
        }

        public async Task CreateAuthTokenAsync(string userName)
        {
            var user = await _userRepository.GetWhere(x => x.UserName == userName)
                .Include(x => x.Authentication).FirstOrDefaultAsync();

            var input = new UpdateTokenInput();
            input.Created = DateTime.Now;
            input.Expires = DateTime.Now.AddMinutes(TokenMinutes);
            input.AccessToken = GenerateJWTToken(user, _config["Jwt:SecretKey"], input.Expires);
            input.RefreshToken = GenerateJWTToken(user, "Super_Pushka_Raketa_Turba_Boost", DateTime.Now.AddDays(30));
            
            if (!user.AuthenticationId.HasValue)
            {
                await CreateNewTokens(user, input);
            }
            else
            {
                await UpdateTokens(user,input);
            }
        }

        public async Task<User> AuthenticateUserAsync(UserLogin loginCredentials, Detect detect, HttpContext httpContext)
        {
            var userByLogin = await _userRepository
                .GetWhere(x => x.UserName == loginCredentials.UserName && x.ActiveKey.IsActive)
                .Include(x => x.Authentication).Include(x => x.ActiveKey).Include(x => x.Detects)
                
                .FirstOrDefaultAsync();

            if (userByLogin == null)
                throw new EntityNotFoundException("User not found");

            if (!HashService.Verify(loginCredentials.Password, userByLogin.Password))
                throw new EntityNotFoundException("Login or Password Was Wrong");

            await DetectUser(detect, userByLogin, httpContext);
            
            return _mapper.Map<User>(userByLogin);
        }


        private async Task DetectUser(Detect detect, User user, HttpContext httpContext)
        {
            if (IsReal(detect, user))
                return;

            user.ActiveKey.IsActive = false;
            user.Detects.Add(detect);
            await _userRepository.UpdateAsync(user);
            var local = $"{LocalAddress}{httpContext.Connection.LocalPort}/Login/ConfirmIdentity/?Key={user.ActiveKey.Key}";
            await _emailService.SendEmailAsync(user.Email, "Identity", local);
            throw new ForbiddenException("Verify your identity. Check your Email ");
        }

        public bool IsReal(Detect detect, User user)
        {
            foreach (var item in user.Detects)
            {
                if (detect.Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        private async Task CreateNewTokens(User user, UpdateTokenInput input)
        {
           var auth = _mapper.Map<Authentication>(input);
           user.Authentication = auth;
           await _userRepository.UpdateAsync(user);
        }

        private async Task UpdateTokens(User user, UpdateTokenInput input)
        {
            // user.Authentication = _mapper.Map<Authentication>(input);
            user.Authentication.AccessToken = input.AccessToken;
            user.Authentication.RefreshToken = input.RefreshToken;
            user.Authentication.Created = input.Created;
            user.Authentication.Expires = input.Expires;
            await _userRepository.UpdateAsync(user);
        }

        private string GenerateJWTToken(User user, string secretWord, DateTime expire)
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