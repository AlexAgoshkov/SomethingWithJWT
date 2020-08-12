using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Models
{
    public class UserChat
    {
        public int ChatId { get; set; }
        [JsonIgnore]
        public virtual Chat Chat { get; set; }

        public int UserId { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }

        public bool IsPrivateMask { get; set; }
    }
}