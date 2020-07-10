using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using MySocNet.Models;
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
    }
}