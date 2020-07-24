using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Models
{
    public class UserMessage
    {
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public int MessageId { get; set; }

        public virtual Message Message { get; set; } 

        public bool IsRead { get; set; }
    }
}