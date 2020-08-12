using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Models
{
    public class ChatMembers
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public int ChatId { get; set; }

        public Chat Chat { get; set; }

        public bool IsUserJoined { get; set; }
    }
}
