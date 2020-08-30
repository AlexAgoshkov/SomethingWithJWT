using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Encodings;
using MySocNet.Enums;
using MySocNet.Exceptions;
using MySocNet.InputData;
using MySocNet.Models;
using MySocNet.Response;
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

        public async Task<PaginatedResponse<User>> GetPaginatedUsers(SearchUserInput userInput)
        {
            IQueryable<User> query = null;

            if (!string.IsNullOrWhiteSpace(userInput.Search))
            {
                query = _userRepository.GetWhere(x =>
                    x.FirstName.ToUpper().Contains(userInput.Search) ||
                    x.SurName.ToUpper().Contains(userInput.Search));
            }

            query = GetSortedQuery(userInput, query);

            int totalCount = await query.CountAsync();

            var result = await query.Skip(userInput.Skip).Take(userInput.Take).ToListAsync();

            return new PaginatedResponse<User>(totalCount, result);
        }

        public async Task<User> ChangeRole(int userId, string role)
        {
            var user = await _userRepository.GetWhere(x => x.Id == userId).FirstOrDefaultAsync() ??
                 throw new EntityNotFoundException("User not found");

            user.UserRole = role;
            await _userRepository.UpdateAsync(user);
            return user;
        }

        public async Task<UserResponse> AddImageToUser(Image image, int userId)
        {
            var user = await _userRepository.GetWhere(x => x.Id == userId)
                .Include(x => x.ProfileImage).FirstOrDefaultAsync();
            if (user == null)
                throw new EntityNotFoundException("User not found");

            if (user != null && image != null)
            {
                user.ProfileImage = image;
                await _userRepository.UpdateAsync(user);
            }
           
            return _mapper.Map<UserResponse>(user);
        }

        private IQueryable<User> GetSortedQuery(SearchUserInput userInput, IQueryable<User> query)
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
                .Include(x => x.Authentication).FirstOrDefaultAsync() ??
                throw new EntityNotFoundException("RefreshToken not found");
        }
    }
}