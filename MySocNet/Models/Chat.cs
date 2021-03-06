﻿using MySocNet.Enums;
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

        public bool IsPrivate { get; set; }

        public bool IsReadOnly { get; set; }

        public ChatType ChatType { get; set; }

        public int? ImageId { get; set; }

        public virtual Image ChatImage { get; set; }
        [JsonIgnore]
        public virtual ICollection<Message> Messages { get; set; }
        [JsonIgnore]
        public virtual ICollection<UserChat> UserChats { get; set; }
        [JsonIgnore]
        public virtual ICollection<ChatMembers> ChatMembers { get; set; }
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
