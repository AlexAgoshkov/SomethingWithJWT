using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MySocNet.Models;
using MySocNet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services
{
    public class ImageService : IImageService
    {
        private readonly IRepository<Image> _imageRepository;

        public ImageService(IRepository<Image> imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public async Task<Image> AddImageAsync(IFormFile input)
        {
            Image result = null;
            var uploads = Path.Combine("Images");
            if(input != null && input.Length > 0)
            {
                using (var fileStream = new FileStream(Path.Combine(uploads, input.FileName), FileMode.Create))
                {
                    await input.CopyToAsync(fileStream);
                    result = new Image { ImagePath = $"{uploads}/{input.FileName}" };
                    await _imageRepository.AddAsync(result);
                }
            }
            return result;
        }

        public async Task<string> UploadImageAsync(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            byte[] byData = new byte[fs.Length];
            await fs.ReadAsync(byData, 0, byData.Length);

            return Convert.ToBase64String(byData);
           // var imgSrc = String.Format("data:image/jpg;base64,{0}", base64);
            //return base64;
        }

        public async Task<byte[]> GetImageByBytesAsync(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            byte[] byData = new byte[fs.Length];
            await fs.ReadAsync(byData, 0, byData.Length);
            return byData;
        }
    }
}
