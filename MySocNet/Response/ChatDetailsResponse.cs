using MySocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Response
{
    public class ChatDetailsResponse
    {
        public List<User> Users { get; set; }

        public string ChatName { get; set; }

        public string Image { get; set; }
    }
}
