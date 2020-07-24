using MySocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Response
{
    public class MessageResponse
    {
        public string Text { get; set; }

        public DateTime Time { get; set; }

        public int SenderId { get; set; }

        public int ImageId { get; set; }

        public int ChatId { get; set; }
    }
}
