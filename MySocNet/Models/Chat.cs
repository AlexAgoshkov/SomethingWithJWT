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

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<UserChat> UserChats { get; set; }

        public string ChatName { get; set; }

        public Chat()
        {
            Messages = new List<Message>();
        }
    }
}
