using MySocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Input
{
    public class InputChatCreate
    {
        public string ChatName { get; set; } 

        public int ReceiverId { get; set; }
    }
}
