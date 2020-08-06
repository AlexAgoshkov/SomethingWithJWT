using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySocNet.Enums;
using MySocNet.Models;
using MySocNet.Services.Interfaces;
using NLog.Filters;
using NLog.Fluent;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


namespace MySocNet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImageController : ApiControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IImageService _imageService;
        private readonly IRepository<MySocNet.Models.Image> _imageRepository;
        private readonly IUserService _userService;
        private readonly ILogger<ImageController> _logger;

        public ImageController(
            IRepository<User> userRepository,
            IRepository<MySocNet.Models.Image> imageRepository,
            IUserService userService,
            IChatService chatService,
            IImageService imageService,
            ILogger<ImageController> logger) : base(userRepository)
        {
            _chatService = chatService;
            _imageService = imageService;
            _userService = userService;
            _imageRepository = imageRepository;
            _logger = logger;
        }

        [HttpPost("AddImageToChat")]
        public async Task<IActionResult> AddImageToChat(IFormFile image, Filters filters, int chatId)
        {
            if (!_imageService.IsImage(image))
                return BadRequest("Wrong Image Formate");

            try
            {
                var pic = await _imageService.UploadAsync(image, filters);
                var chat = await _chatService.AddImageToChatAsync(pic, chatId);
                _logger.LogInformation($"Image Id: {pic.Id} Was Added to Chat Id: {chatId}");
                return JsonResult(chat);
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

            var imageBase64 = await _imageService.DownloadAsync(image.CroppedImagePath);
            _logger.LogInformation($"Image Id: {image.Id} Was Printed");
            return JsonResult(imageBase64);
        }

        [HttpPost("AddImageToUser")]
        public async Task<IActionResult> AddImageToUser(IFormFile image, Filters filters, int userId)
        {
            if (!_imageService.IsImage(image))
                return BadRequest("Wrong Image Formate");

            try
            {
                var pic = await _imageService.UploadAsync(image, filters);
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