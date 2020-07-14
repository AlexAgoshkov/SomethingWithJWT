using MySocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Input
{
    public class SendMessageInput
    {
        public int ChatId { get; set; }

        public string Message { get; set; }
    }
}
