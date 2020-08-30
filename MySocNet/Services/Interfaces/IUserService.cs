using MySocNet.InputData;
using MySocNet.Models;
using MySocNet.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MySocNet.Services.Interfaces
{
    public interface IUserService
    {
        Task<PaginatedResponse<User>> GetPaginatedUsers(SearchUserInput userInput);

        Task<User> GetUserByRefreshTokenAsync(string refreshToken);

        Task<UserResponse> AddImageToUser(Image image, int userId);

        Task<User> ChangeRole(int userId, string role);
    }
}