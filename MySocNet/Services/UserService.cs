using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Encodings;
using MySocNet.Enums;
using MySocNet.InputData;
using MySocNet.Models;
using MySocNet.OutPutData;
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
        private readonly MyDbContext _myDbContext;
        private readonly IMapper _mapper;

        public UserService(MyDbContext myDbContext, IMapper mapper)
        {
            _myDbContext = myDbContext;
            _mapper = mapper;
        }

        public IQueryable<User> GetUsersAsync(Expression<Func<User, bool>> filter)
        {
            return _myDbContext.Users.Where(filter);
        }
        public IQueryable<User> GetAll()
        {
            return _myDbContext.Users.AsQueryable();
        }

        public async Task<IQueryable<User>> GetSortedQuery(SearchUserInput userInput, IQueryable<User> query)
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

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _myDbContext.Users
                .Include(x => x.Friends)
                .Include(x => x.Authentication)
                .Include(x => x.ActiveKey)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task CreateUserAsync(User user)
        {
            foreach (var item in _myDbContext.Users)
            {
                if (user.UserName == item.UserName)
                {
                    return;
                }
            }

            user.Password = HashService.Hash(user.Password);
            user.UserRole = "User";
            await _myDbContext.Users.AddAsync(user);
            await _myDbContext.SaveChangesAsync();
        }

        public async Task AddFriendToUserAsync(int userId, int friendId)
        {
            var user = await _myDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

            var friend = new Friend { UserAddedId = friendId, UserId = userId };

            user.Friends.Add(friend);
            _myDbContext.Users.Update(user);
            await _myDbContext.SaveChangesAsync();
        }

        public async Task<IList<User>> GetFriendListAsync(int userId)
        {
            var friendIds = await _myDbContext.Friends
            .Where(x => x.UserId == userId || x.Id == userId)
            .Select(x => x.UserId == userId ? x.Id : x.UserId)
            .ToListAsync();

            var friends = await _myDbContext.Users
                .Where(x => friendIds.Contains(x.Id))
                .ToListAsync();

            return friends;
        }

        public async Task UpdateUserAsync(User input)
        {
            _myDbContext.Users.Update(input);
            await _myDbContext.SaveChangesAsync();
        }

        public async Task AddTokenToUserAsync(User userInfo, Authentication auth)
        {
            var user = await _myDbContext.Users.Include(x => x.Authentication).FirstOrDefaultAsync(x => x.Id == userInfo.Id);

            if (userInfo.AuthenticationId.HasValue)
            { 
                user.Authentication = auth;
            }
            _myDbContext.Users.Update(user);
            await _myDbContext.SaveChangesAsync();
        }

        public async Task RefreshTokenAsync(User userInfo, string token)
        {
            var user = await _myDbContext.Users.Include(x => x.Authentication).FirstOrDefaultAsync(x => x.Id == userInfo.Id);

            if (userInfo.AuthenticationId.HasValue)
            {
                user.Authentication.Created = DateTime.Now;
                user.Authentication.AccessToken = token;
                _myDbContext.Users.Update(user);
                await _myDbContext.SaveChangesAsync();
            }
        }

        public async Task<User> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _myDbContext.Users.Include(x => x.Authentication).FirstOrDefaultAsync(x => x.Authentication.RefreshToken == refreshToken);
        }

        public async Task<User> GetUserByUserNameAsync(string username)
        {
            return await _myDbContext.Users.Include(x => x.ActiveKey).
                                            Include(x => x.Authentication).
                                            Include(x => x.Friends).
                                            FirstOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _myDbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task ConfirmEmailAsync(string Key)
        {
            var user = await _myDbContext.Users.Include(x => x.ActiveKey).FirstOrDefaultAsync(x => x.ActiveKey.Key == Key);
            var time = DateTime.Now - user.ActiveKey.Created;

            if (!user.ActiveKey.IsActive && time.Hours < 1)
            {
                user.ActiveKey.IsActive = true;
                _myDbContext.Update(user);
                await _myDbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveUserByIdAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            _myDbContext.Remove(user);
            await _myDbContext.SaveChangesAsync();
        }
    }
}

