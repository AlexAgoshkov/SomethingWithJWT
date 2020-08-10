using MySocNet.InputData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Input
{
    public class SearchChatsInput : PaginatedInput
    {
        public string Search { get; set; }

        public bool IsAscending { get; set; }
    }
}
