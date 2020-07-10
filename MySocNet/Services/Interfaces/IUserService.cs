using MySocNet.InputData;
using MySocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MySocNet.Services.Interfaces
{
    public interface IUserService
    {
        IQueryable<User> GetSortedQuery(SearchUserInput userInput, IQueryable<User> query);
        Task<User> GetUserByRefreshTokenAsync(string refreshToken);//--
    }
}