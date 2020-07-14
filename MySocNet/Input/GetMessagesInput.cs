using MySocNet.InputData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Input
{
    public class GetMessagesInput : PaginatedInput
    { 
        public int ChatId { get; set; }
    }
}
