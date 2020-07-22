using MySocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Response
{
    public class GetNewMessageResponse
    {
        public IList<Message> Messages { get; set; }

        public int TotalCount { get; set; }
    }
}
