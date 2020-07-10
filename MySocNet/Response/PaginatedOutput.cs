using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Response
{
    public class PaginatedOutput<T>
    {
        public int TotalCount { get; set; }

        public List<T> Data { get; set; }

        public PaginatedOutput(int totalCount, List<T> data)
        {
            TotalCount = totalCount;
            Data = data;
        }
    }
}
