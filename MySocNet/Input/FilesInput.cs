using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Input
{
    public class FilesInput
    {
        public IFormFile Avatar { get; set; }
    }
}
