using MySocNet.Enums;
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

        public int[] Ids { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsReadOnly { get; set; }

        public ChatType ChatType { get; set; }
    }
}
