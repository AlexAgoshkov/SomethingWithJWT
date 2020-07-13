using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Response
{
    public class PaginatedResponse<T>
    {
        public int TotalCount { get; set; }

        public List<T> Data { get; set; }

        public PaginatedResponse(int totalCount, List<T> data)
        {
            TotalCount = totalCount;
            Data = data;
        }
    }
}
