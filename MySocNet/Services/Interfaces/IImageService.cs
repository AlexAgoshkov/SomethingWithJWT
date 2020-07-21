using Microsoft.AspNetCore.Http;
using MySocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(string path);
        Task<Image> AddImageAsync(IFormFile input);
        Task<byte[]> GetImageByBytesAsync(string path);
    }
}
