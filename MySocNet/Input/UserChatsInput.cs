using MySocNet.InputData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Input
{
    public class UserChatsInput : PaginatedInput
    {
        public bool IsPrivateMask { get; set; }
    }
}
