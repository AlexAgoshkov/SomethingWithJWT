using Microsoft.EntityFrameworkCore;
using MySocNet.Models;
using MySocNet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services
{
    public class FriendService : IFriendService
    {
        private readonly MyDbContext _myDbContext;
        
        public FriendService(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
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
    }
}