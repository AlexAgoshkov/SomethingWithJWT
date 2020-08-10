using MySocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Input
{
    public class UpdateTokenInput
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime Created { get; set; }

        public DateTime Expires { get; set; }

    }
}
