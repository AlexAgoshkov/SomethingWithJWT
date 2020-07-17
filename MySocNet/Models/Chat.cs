using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }

        public int? ImageId { get; set; }

        public virtual Image ChatImage { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserChat> UserChats { get; set; }

        public string ChatName { get; set; }

        public int? ChatOwnerId { get; set; }

        [JsonIgnore]
        public User ChatOwner { get; set; }

        public Chat()
        {
            Messages = new List<Message>();
        }
    }
}
