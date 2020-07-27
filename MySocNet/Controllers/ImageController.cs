using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySocNet.Models;
using MySocNet.Services.Interfaces;

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

        public ImageController(
            IRepository<User> userRepository, 
            IUserService userService,
            IChatService chatService, 
            IImageService imageService) : base(userRepository)
        {
            _chatService = chatService;
            _imageService = imageService;
            _userService = userService;
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
                return JsonResult(pic);
            }
            catch (Exception ex)
            {
               // _logger.LogInformation(ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("GetImageById")]
        public async Task<IActionResult> GetImage(int fileId)
        {
            var image = await _imageRepository.GetByIdAsync(fileId);

            if (image == null)
                return NotFound();

            var imageBase64 = await _imageService.DownloadAsync(image.ImagePath);

            return JsonResult(imageBase64);
        }

        [HttpPost("AddImageToUser")]
        public async Task<IActionResult> AddImageToUser(IFormFile image, int userId)
        {
            var pic = await _imageService.UploadAsync(image);
            var user = await _userService.AddImageToUser(pic, userId);
            return JsonResult(user);
        }
    }
}
