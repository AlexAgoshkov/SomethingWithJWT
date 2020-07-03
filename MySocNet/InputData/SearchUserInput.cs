using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.InputData
{
    public class SearchUserInput
    {
        public string Name { get; set; }

        public int Skip { get; set; } = 0;

        public int Take { get; set; } = 10;

        public bool IsSort { get; set; }

        public string OrderKey { get; set; }
    }
}
