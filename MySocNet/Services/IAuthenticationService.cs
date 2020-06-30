using MySocNet.InputData;
using MySocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services
{
    public interface IAuthenticationService
    {
        Task<User> AuthenticateUserAsync(UserLogin loginCredentials);

        Task<string> GenerateJWTTokenAsync(User user, string secretWord, DateTime expire);

        Task CreateTokenPairAsync(User userInfo, DateTime created, DateTime expires, string accessToken, string refreshToken);

        Task CreateActiveKeyAsync(string key);

        Task<ActiveKey> GetActiveKeyByNameAsync(string key);

        Task AddActiveKeyToUserAsync(int userId, int keyId);

        string GetRandomString();
    }
}
