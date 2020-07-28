using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MySocNet.Hubs;
using MySocNet.Input;
using MySocNet.Models;
using MySocNet.Services.Interfaces;
using RestSharp;

namespace MySocNet.Controllers
{
    public class SignalController : Controller
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Chat> _chatRepository;
        private readonly IChatService _chatService;
       
        public SignalController(
            IRepository<User> userRepository, 
            IRepository<Chat> chatRepository,
            IChatService chatService
            )
        {
            _userRepository = userRepository;
            _chatRepository = chatRepository;
            _chatService = chatService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int userId, int chatId)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);
            if (chat == null)
                return NotFound("Chat not found");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            ViewData["Name"] = $"{user.FirstName} {user.SurName}";
            ViewData["Chat"] = chatId;
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginSignalRModel model)
        {
            var user = await _userRepository.GetWhere(x => x.Authentication.AccessToken == model.AccessToken)
                .Include(x => x.Authentication).FirstOrDefaultAsync();
            int userId = user.Id;
           
            if (user != null)
            {
                Response.Cookies.Append("key", model.AccessToken);
                return RedirectToAction("Index", "Signal", 
                    new { userId = userId, chatId = model.ChatId});
            }
            return View(model);
        }
    }
}
