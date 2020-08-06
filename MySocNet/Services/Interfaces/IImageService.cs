using Microsoft.AspNetCore.Http;
using MySocNet.Enums;
using MySocNet.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services.Interfaces
{
    public interface IImageService
    {
        Task<string> DownloadAsync(string path);
        Task<byte[]> GetImageByBytesAsync(string path);
        Task<Models.Image> UploadAsync(IFormFile input, Filters filters);
        bool IsImage(IFormFile file);
    }
}
