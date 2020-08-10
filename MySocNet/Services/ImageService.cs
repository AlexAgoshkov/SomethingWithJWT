using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using MySocNet.Enums;
using MySocNet.Exceptions;
using MySocNet.Models;
using MySocNet.Services.Interfaces;
using NLog.Filters;
using NLog.Time;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services
{
    public class ImageService : IImageService
    {
        private readonly IRepository<Models.Image> _imageRepository;
        private const string ImageFolderPath = @"D:/Source/SomethingWithJWT/MySocNet/Images/";
        private const string ResizeFolderPath = @"D:/Source/SomethingWithJWT/MySocNet/CroppedImages/";
        private const int ImageSize = 320;

        public ImageService(IRepository<Models.Image> imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public async Task<Models.Image> UploadAsync(IFormFile input, Filters filters, string fileName)
        {
            var imageName = $"{fileName}_{input.FileName}";
            await CreateImage(input.OpenReadStream(), imageName, filters);
            var image = new Models.Image
            {
                ImagePath = $"{ImageFolderPath}{imageName}",
                CroppedImagePath = $"{ResizeFolderPath}{imageName}"
            };
            return image;
        }


        public async Task<string> DownloadAsync(string path)
        {
            if (!File.Exists(path))
                throw new EntityNotFoundException("File not found");

            using (var fs = new FileStream(path, FileMode.Open))
            {
                byte[] byData = new byte[fs.Length];
                await fs.ReadAsync(byData, 0, byData.Length);

                return Convert.ToBase64String(byData);
            }
        }

        public async Task<byte[]> GetImageByBytesAsync(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                byte[] byData = new byte[fs.Length];
                await fs.ReadAsync(byData, 0, byData.Length);
                return byData;
            }
        }

        public bool IsImage(IFormFile file)
        {
            bool result = false;

            if (file.FileName.EndsWith(".jpg")
                || file.FileName.EndsWith(".png")
                || file.FileName.EndsWith(".jpeg"))
            {
                result = true;
            }

            return result;
        }

        private int GetWidth(double w, double h)
        {
            return (int)(ImageSize / (h / w));
        }
        private int GetHeight(double w, double h)
        {
            return (int)(ImageSize / (w / h));
        }

        private async Task ResizeImage(string filePath, string imageName)
        {
            using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(filePath))
            {
                if (image.Height > image.Width)
                {
                    image.Mutate(x => x.Resize(GetWidth(image.Width, image.Height), ImageSize));
                }
                else
                {
                    image.Mutate(x => x.Resize(ImageSize, GetHeight(image.Width, image.Height)));
                }
                
                await image.SaveAsync($"{ResizeFolderPath}{imageName}");  
            }
        }

        private async Task<SixLabors.ImageSharp.Image> CreateImage(Stream stream, string imageName, Filters filters)
        {
            string filePath = $"{ImageFolderPath}{imageName}";

            using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(stream))
            {
                AddFiltres(filters, image);
                await image.SaveAsync(filePath);
                await ResizeImage(filePath, imageName);
                return image;
            }
        }
        private void AddFiltres(Filters filters, SixLabors.ImageSharp.Image image)
        {
            switch (filters)
            {
                case Filters.AutoOrient:
                    image.Mutate(x => x.AutoOrient());
                    break;
                case Filters.AdaptiveThreshold:
                    image.Mutate(x => x.AdaptiveThreshold());
                    break;
                case Filters.BlackWhite:
                    image.Mutate(x => x.BlackWhite());
                    break;
                case Filters.BoxBlur:
                    image.Mutate(x => x.BoxBlur());
                    break;
                case Filters.Polaroid:
                    image.Mutate(x => x.Polaroid());
                    break;
                case Filters.DetectEdges:
                    image.Mutate(x => x.DetectEdges());
                    break;
                case Filters.Dither:
                    image.Mutate(x => x.Dither());
                    break;
                case Filters.Flip:
                    image.Mutate(x => x.Flip(FlipMode.Horizontal));
                    break;
                case Filters.GaussianBlur:
                    image.Mutate(x => x.GaussianBlur());
                    break;
                case Filters.Grayscale:
                    image.Mutate(x => x.Grayscale());
                    break;
                case Filters.Kodachrome:
                    image.Mutate(x => x.Kodachrome());
                    break;
                case Filters.OilPaint:
                    image.Mutate(x => x.OilPaint());
                    break;
                default:
                    break;
            }
        }
    }
}
