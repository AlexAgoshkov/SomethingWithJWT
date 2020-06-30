using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MySocNet.Models;
using MySocNet.OutPutData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public async Task<IList<User>> GetUsersAsync()
        {
            return await _myDbContext.Users.Include(x => x.Friends).
                                            Include(x => x.Authentication).
                                            Include(x => x.ActiveKey).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _myDbContext.Users.Include(x => x.Friends).
                                            Include(x => x.Authentication).
                                            Include(x => x.ActiveKey).
                                            FirstOrDefaultAsync(x => x.UserId == id);
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
            var user = await _myDbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);

            var friend = new Friend { UserAddedId = friendId, UserID = userId };

            user.Friends.Add(friend);
            _myDbContext.Users.Update(user);
            await _myDbContext.SaveChangesAsync();
        }

        public async Task<IList<UserOutPut>> GetFriendListAsync(int userId)
        {
            var user = await _myDbContext.Users.Include(x => x.Friends).FirstOrDefaultAsync(x => x.UserId == userId);
            var list = new List<UserOutPut>();
            var friends = user.Friends;

            foreach (var item in user.Friends)
            {
                var friend = await GetUserByIdAsync(item.UserAddedId);
                var outPut = _mapper.Map<UserOutPut>(friend);
                list.Add(outPut);
            }

            return list;
        }

        public async Task<IList<UserOutPut>> GetPaddingList(int userId, int skip, int take)
        {
            var friends = await GetFriendListAsync(userId);
            return friends.Skip(skip).Take(take).ToList();
        }

        public async Task UpdateUserAsync(User input)
        {
            _myDbContext.Users.Update(input);
            await _myDbContext.SaveChangesAsync();
        }

        public async Task AddTokenToUserAsync(User userInfo, Authentication auth)
        {
            var user = await _myDbContext.Users.Include(x => x.Authentication).FirstOrDefaultAsync(x => x.UserId == userInfo.UserId);

            if (userInfo.AuthenticationId.HasValue)
            {
                user.Authentication = auth;
            }
            _myDbContext.Users.Update(user);
            await _myDbContext.SaveChangesAsync();
        }

        public async Task RefreshTokenAsync(User userInfo, string token)
        {
            var user = await _myDbContext.Users.Include(x => x.Authentication).FirstOrDefaultAsync(x => x.UserId == userInfo.UserId);

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

        public async Task<IList<User>> GetUsersBySurname(string surname)
        {
            return await _myDbContext.Users.Where(x => x.SurName == surname).ToListAsync();
        }

        public async Task<IList<User>> GetUsersByName(string name)
        {
            return await _myDbContext.Users.Where(x => x.FirstName == name).ToListAsync();
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

        public async Task<int> GetTotalUserCount(User user, int take)
        {
            return user.Friends.Count - take;
        }
    }
}