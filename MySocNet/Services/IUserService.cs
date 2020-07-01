    using MySocNet.Models;
using MySocNet.OutPutData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services
{
    public interface IUserService
    {
        Task<IList<User>> GetUsersAsync();

        Task CreateUserAsync(User user);

        Task AddFriendToUserAsync(int userId, int friendId);

     //   Task<IList<UserOutPut>> GetPaddingList(int userId, int skip, int take);

        Task<IList<UserOutPut>> GetFriendListAsync(int userId);

        Task<User> GetUserByIdAsync(int id);

        Task AddTokenToUserAsync(User userInfo, Authentication auth);

        Task RefreshTokenAsync(User userInfo, string token);

        Task<User> GetUserByRefreshTokenAsync(string refreshToken);

        Task<User> GetUserByUserNameAsync(string username);

        Task<User> GetUserByEmail(string email);

        Task<IList<User>> GetUsersBySurname(string surname);

        Task<IList<User>> GetUsersByName(string name);

        Task UpdateUserAsync(User input);

        Task RemoveUserByIdAsync(int id);

        Task ConfirmEmailAsync(string Key);

        Task<int> GetTotalUserCount(User user, int take);
    }
}