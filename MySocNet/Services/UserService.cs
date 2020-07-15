using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Encodings;
using MySocNet.Enums;
using MySocNet.InputData;
using MySocNet.Models;
using MySocNet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading.Tasks;

namespace MySocNet.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        
        public UserService(
            IMapper mapper, 
            IRepository<User> userRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public IQueryable<User> GetSortedQuery(SearchUserInput userInput, IQueryable<User> query)
        {
            switch (userInput.OrderKey)
            {
                case SearchUserOrderKey.FirstName:
                    query = userInput.IsAscending ? query.OrderBy(x => x.FirstName) : query.OrderByDescending(x => x.FirstName);
                    break;
                case SearchUserOrderKey.LastName:
                    query = userInput.IsAscending ? query.OrderBy(x => x.SurName) : query.OrderByDescending(x => x.SurName);
                    break;
            }

            return query;
        }

        public async Task<User> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _userRepository.GetWhere(x => x.Authentication.RefreshToken == refreshToken)
                .Include(x => x.Authentication).FirstOrDefaultAsync();
        }
    }
}