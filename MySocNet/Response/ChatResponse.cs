using MySocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Response
{
    public class ChatResponse
    {
        public int Id { get; set; }

        public string ChatName { get; set; }

        public int UnReadMessageCount { get; set; }

        public string LastMessage { get; set; }

        public string SenderName { get; set; }

        public DateTime DateTime { get; set; }
    }
}
