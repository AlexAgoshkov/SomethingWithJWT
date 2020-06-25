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
        Task<User> AuthenticateUser(UserLogin loginCredentials);

        Task<string> GenerateJWTToken(User user, string secretWord, DateTime expire);

        Task CreateTokenPairAsync(User userInfo, DateTime created, DateTime expires, string accessToken, string refreshToken);
    }
}
