using MySocNet.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.InputData
{
    public class SearchUserInput : PaginatedInput
    {
        public string Search { get; set; }

        public bool IsAscending { get; set; }

        public SearchUserOrderKey OrderKey { get; set; }
    }

    public class PaginatedInput
    {
        [Range(0, int.MaxValue)]
        public int Skip { get; set; } = 0;

        [Range(1, 100)]
        public int Take { get; set; } = 10;
    }
}
