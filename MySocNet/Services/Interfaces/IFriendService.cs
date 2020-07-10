using MySocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services.Interfaces
{
    public interface IFriendService
    {
        Task AddFriendToUserAsync(int userId, int friendId);

        Task<IList<User>> GetFriendListAsync(int userId);
    }
}
