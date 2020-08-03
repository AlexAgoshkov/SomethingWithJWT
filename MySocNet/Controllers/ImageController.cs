using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySocNet.Models;
using MySocNet.Services.Interfaces;
using NLog.Fluent;

namespace MySocNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImageController : ApiControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IImageService _imageService;
        private readonly IRepository<Image> _imageRepository;
        private readonly IUserService _userService;
        private readonly ILogger<ImageController> _logger;

        public ImageController(
            IRepository<User> userRepository, 
            IUserService userService,
            IChatService chatService, 
            IImageService imageService,
            ILogger<ImageController> logger) : base(userRepository)
        {
            _chatService = chatService;
            _imageService = imageService;
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("AddImageToChat")]
        public async Task<IActionResult> AddImageToChat(IFormFile image, int chatId)
        {
            if (!image.FileName.EndsWith(".jpg"))
                return BadRequest();

            try
            {
                var pic = await _imageService.UploadAsync(image);
                await _chatService.AddImageToChatAsync(pic, chatId);
                _logger.LogInformation($"Image Id: {pic.Id} Was Added to Chat Id: {chatId}");
                return JsonResult(pic);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetImageById")]
        public async Task<IActionResult> GetImage(int fileId)
        {
            var image = await _imageRepository.GetByIdAsync(fileId);

            if (image == null)
                return NotFound();

            var imageBase64 = await _imageService.DownloadAsync(image.ImagePath);
            _logger.LogInformation($"Image Id: {image.Id} Was Printed");
            return JsonResult(imageBase64);
        }

        [HttpPost("AddImageToUser")]
        public async Task<IActionResult> AddImageToUser(IFormFile image, int userId)
        {
            try
            {
                var pic = await _imageService.UploadAsync(image);
                var user = await _userService.AddImageToUser(pic, userId);
                _logger.LogInformation($"Image Id: {pic.Id} Was Added to User Id {userId}");
                return JsonResult(user);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}