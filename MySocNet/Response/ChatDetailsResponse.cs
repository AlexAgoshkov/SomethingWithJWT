using MySocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Response
{
    public class ChatDetailsResponse
    {
        public List<string> Users { get; set; }

        public string ChatName { get; set; }
    }
}
