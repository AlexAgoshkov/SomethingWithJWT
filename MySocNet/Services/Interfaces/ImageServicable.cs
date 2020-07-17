using Microsoft.AspNetCore.Http;
using MySocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services.Interfaces
{
    public interface ImageServicable
    {
        Task<string> UploadImageAsync(string path);
        Task<Image> AddImageAsync(IFormFile input);
    }
}
