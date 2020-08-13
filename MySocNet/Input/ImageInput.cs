using Microsoft.AspNetCore.Http;
using MySocNet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Input
{
    public class ImageInput
    {
        public IFormFile Image { get; set; }
        public Filters Filters { get; set; }
    }
}
