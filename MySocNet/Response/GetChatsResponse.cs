using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MySocNet.Response
{
    public class GetChatsResponse
    {
       public IList<ChatResponse> ChatResponses { get; set; }
       
       public int TotalCount { get; set; }
    }
}
