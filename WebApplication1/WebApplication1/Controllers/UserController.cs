using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _myDbContext;

        public UserController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }

        [HttpGet("GetAllUsers")]
       
        public async Task<IActionResult> GetAsync()
        {
            var users = await _myDbContext.Users.ToListAsync();
            return Ok(new { users });
        }

    }
}
