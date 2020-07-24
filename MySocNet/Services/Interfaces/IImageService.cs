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
        Task<string> DownloadAsync(string path);
        Task<Image> UploadAsync(IFormFile input);
        Task<byte[]> GetImageByBytesAsync(string path);
    }
}
