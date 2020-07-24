using MySocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Response
{
    public class GetNewMessageResponse
    {
        public List<MessageResponse> Messages { get; set; }

        public int TotalCount { get; set; }

        public GetNewMessageResponse(List<MessageResponse> messages, int totalCount)
        {
            Messages = messages;
            TotalCount = totalCount;
        }
    }
}
