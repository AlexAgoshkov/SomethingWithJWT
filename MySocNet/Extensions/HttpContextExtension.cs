using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using MySocNet.Models;
using MySocNet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Extensions
{
    public static class HttpContextExtension
    {
        public async static Task<string> GetAccessToken(this HttpContext httpContext)
        {
            return await httpContext.GetTokenAsync("access_token");
        }

        public async static Task<User> GetUserByAccessTokenAsync(this HttpContext httpContext, IRepository<User> userRepository)
        {
            var accessToken = await httpContext.GetTokenAsync("access_token");
            var user = await userRepository.FirstOrDefaultAsync(x => x.Authentication.AccessToken == accessToken)
                   ?? throw new UnauthorizedAccessException();
            return user;
        }
    }
}