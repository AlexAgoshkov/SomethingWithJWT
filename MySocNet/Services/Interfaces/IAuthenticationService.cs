using Microsoft.AspNetCore.Http;
using MySocNet.InputData;
using MySocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Authentication> CreateAuthTokenAsync(string userName);

        Task<User> AuthenticateUserAsync(UserLogin loginCredentials);

        Task DetectUserAsync(Detect detect, User user, string confirmlink);
    }
}
