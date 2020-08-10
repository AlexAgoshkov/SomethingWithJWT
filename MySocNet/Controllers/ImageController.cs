using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySocNet.Enums;
using MySocNet.Exceptions;
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
        
        public ImageController(
            IRepository<User> userRepository,
            IRepository<MySocNet.Models.Image> imageRepository,
            IUserService userService,
            IChatService chatService,
            IImageService imageService
            ) : base(userRepository)
        {
            _chatService = chatService;
            _imageService = imageService;
            _userService = userService;
            _imageRepository = imageRepository;
        }

        [HttpPost("AddImageToChat")]
        
        public async Task<IActionResult> AddImageToChat(IFormFile image, Filters filters, int chatId)
        {
            if (!_imageService.IsImage(image))
                throw new EntityNotFoundException("Image must be .jpg .png. .jpeg");

                var chatFileName = $"Chat_{chatId}";
                var pic = await _imageService.UploadAsync(image, filters, chatFileName);
                var chat = await _chatService.AddImageToChatAsync(pic, chatId);
                return JsonResult(chat);
        }

        [HttpGet("GetImageById")]
        public async Task<IActionResult> GetImage(int fileId)
        {
            var image = await _imageRepository.GetByIdAsync(fileId);
            if (image == null)
                throw new EntityNotFoundException("Image not found");

            var imageBase64 = await _imageService.DownloadAsync(image.CroppedImagePath);
            return JsonResult(imageBase64);   
        }

        [HttpPost("AddImageToUser")]
        [Authorize(Roles = Policies.Admin + "," + Policies.User)]
        public async Task<IActionResult> AddImageToUser(IFormFile image, Filters filters)
        {
            var currentUser = await CurrentUser();

            if (!_imageService.IsImage(image))
                throw new EntityNotFoundException("Image must be .jpg .png. .jpeg");

            var userFileName = $"User_{currentUser.Id}";
            var pic = await _imageService.UploadAsync(image, filters, userFileName);
            var user = await _userService.AddImageToUser(pic, currentUser.Id);
            return JsonResult(user);
        }
    }
}