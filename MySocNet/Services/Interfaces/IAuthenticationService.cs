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
        Task CreateAuthTokenAsync(string userName);

        Task<User> AuthenticateUserAsync(UserLogin loginCredentials, Detect detect, HttpContext httpContext);
    }
}
