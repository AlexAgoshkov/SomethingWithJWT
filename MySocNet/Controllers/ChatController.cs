using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySocNet.Extensions;
using MySocNet.Input;
using MySocNet.InputData;
using MySocNet.Models;
using MySocNet.Response;
using MySocNet.Services.Interfaces;
using Newtonsoft.Json;

namespace MySocNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ChatController : ApiControllerBase
    {
      
        private readonly IChatService _chatService;
        private readonly IImageService _imageService;
        private readonly ILogger<ChatController> _logger;
        private readonly IRepository<Image> _imageRepository;

        public ChatController(
            IRepository<User> userRepository,
            IChatService chatService,
            IImageService imageService,
            ILogger<ChatController> logger,
            IRepository<Image> imageRepository) : base(userRepository)
        {
            _imageService = imageService;
            _chatService = chatService;
            _imageRepository = imageRepository;
            _logger = logger;
        }

        [HttpGet("GetImageById")]
        public async Task<IActionResult> GetImage(int imageId)
        {
            var image = await _imageRepository.GetByIdAsync(imageId);
            if (image == null)
                return BadRequest();
            
            var imageBase64 = await _imageService.UploadImageAsync(image.ImagePath);
            return JsonResult(imageBase64);
        }

        [HttpPost("AddNewUserToChat")]
        public async Task<IActionResult> AddNewUserToChat(UserToChatInput input)
        {
            try
            {
                var response = await _chatService.AddNewUserToChatAsync(input.ChatId, input.UserId);
                return JsonResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }

        [HttpDelete("RemoveUserFromChat")]
        public async Task<IActionResult> RemoveUserFromChat(UserToChatInput input)
        {
            try
            {
                var response = await _chatService.RemoveUserFromChatAsync(input.ChatId, input.UserId);
                return JsonResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }

        [Authorize(Policy = Policies.User)]
        [HttpDelete("RemoveChat")]
        public async Task<IActionResult> RemoveChat(int chatId)
        {
            try
            {
                var user = await CurrentUser();
                var chat = await _chatService.RemoveChatAsync(user.Id, chatId);
                return JsonResult(chat);
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }

        [HttpPost("AddImageToChat")]
        public async Task<IActionResult> AddImageToChat(IFormFile image, int chatId)
        {
            if (!image.FileName.EndsWith(".jpg"))
                return BadRequest();

            try
            {
                var pic = await _imageService.AddImageAsync(image);
                await _chatService.AddImageToChatAsync(pic, chatId);
                return JsonResult(pic);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }  
        }

        [HttpPost("EditChat")]
        public async Task<IActionResult> EditChat(UpdateChatInput input)
        {
            try
            {
                var response = await _chatService.EditChatAsync(input.ChatId, input.ChatName);
                if (response == null)
                    return BadRequest();

                return JsonResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("GetChatDetails")]
        public async Task<IActionResult> GetChatDetails(int chatId)
        {
            try
            {
                var result = await _chatService.GetChatDetailsAsync(chatId);
                if (result.ChatName == null)
                    return BadRequest();

                return JsonResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("GetChats")]
        public async Task<IActionResult> GetChats([FromQuery]PaginatedInput input)
        {
            try
            {
                var user = await CurrentUser();
                var chatList = await _chatService.GetChatsAsync(user.Id, input.Skip, input.Take);
                var response = new GetChatsResponse { ChatResponses = chatList, TotalCount = chatList.Count };
                return JsonResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }

        [HttpPost("CreateChat")]
        public async Task<IActionResult> CreateChat(InputChatCreate input)
        {
            try
            {
                var chatOwner = await CurrentUser();
                var chat = await _chatService.CreateChatAsync(input.ChatName, chatOwner, input.Ids);
                return JsonResult(chat);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("GetAllMessages")]
        public async Task<IActionResult> GetAllMessages([FromQuery] GetMessagesInput input)
        {
            try
            {
                var messages = await _chatService.GetChatHistoryAsync(input.ChatId, input.Skip, input.Take);
                return JsonResult(messages);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("GetNewMessages")]
        public async Task<IActionResult> GetNewMessages([FromQuery]GetMessagesInput input)
        {
            var messages = await _chatService.GetNewMessagesAsync(input.ChatId, input.Skip, input.Take);
            //var user = await CurrentUser();
            return JsonResult(messages);
        }

        [HttpPost("SendMessageToChat")]
        public async Task<IActionResult> SendMessage(SendMessageInput input)
        {
            try
            {
                var messageSender = await CurrentUser();
                var message = await _chatService.SendMessageAsync(input.ChatId, messageSender, input.Message);
                return JsonResult(message);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }
    }
}
