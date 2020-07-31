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
        private readonly IRepository<UserChat> _userChatRepository;
        private readonly IChatService _chatService;
        
        public SignalController(
            IRepository<User> userRepository, 
            IRepository<Chat> chatRepository,
            IRepository<UserChat> userChatRepository,
            IChatService chatService
            )
        {
            _userRepository = userRepository;
            _chatRepository = chatRepository;
            _chatService = chatService;
            _userChatRepository = userChatRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int chatId)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);
            if (chat == null)
                return NotFound("Chat not found");

            ViewData["Chat"] = chatId;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Chats(int userId)
        {
            var test = await _userChatRepository.GetWhere(x => x.UserId == userId).Include(x => x.Chat).Select(x => x.ChatId).ToListAsync();
            List<Chat> chats = await _chatRepository.GetWhere(x => test.Contains(x.Id)).Include(x => x.UserChats).ThenInclude(x => x.User).ToListAsync();

            return View(chats);
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
                return RedirectToAction("Chats", "Signal", new { userId });
            }
            return View(model);
        }
    }
}
