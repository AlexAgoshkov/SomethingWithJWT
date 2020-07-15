using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySocNet.Models;
using MySocNet.Services.Interfaces;
using Newtonsoft.Json;

namespace MySocNet.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        private readonly IRepository<User> _userRepository;

        public ApiControllerBase(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        protected IActionResult JsonResult(object data)
        {
            return new ContentResult
            {
                StatusCode = 200,
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(data)
            };
        }

        protected async Task<User> CurrentUser()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Authentication.AccessToken == accessToken)
                   ?? throw new UnauthorizedAccessException();
            return user;
        }
    }
}
