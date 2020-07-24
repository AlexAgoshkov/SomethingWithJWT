using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Response
{
    public class ChatResponse
    {
        public int Id { get; set; }

        public int? ImageId { get; set; }

        public string ChatName { get; set; }

        public int? ChatOwnerId { get; set; }
    }
}
