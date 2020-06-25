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
            return await _myDbContext.Users.Include(x => x.Friends).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _myDbContext.Users.Include(x => x.Friends).Include(x => x.Authentication).FirstOrDefaultAsync(x => x.UserId == id);
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
            var a = user.Friends;

            foreach (var item in user.Friends)
            {
                var friend = await GetUserByIdAsync(item.UserAddedId);
                var outPut = _mapper.Map<UserOutPut>(friend);
                list.Add(outPut);
            }

            return list;
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
            return await _myDbContext.Users.FirstOrDefaultAsync(x => x.UserName == username);
        }
    }
}
