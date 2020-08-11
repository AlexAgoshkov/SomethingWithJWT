using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Models
{
    public class ChatMessage
    {
        public int? MessageId { get; set; }

        public virtual Message Message { get; set; }

        public int? ChatId { get; set; }

        public virtual Chat Chat { get; set; }
    }
}