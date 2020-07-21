using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Models
{
    public class LastChatData
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }

        public virtual User User { get; set; }

        public int? MessageId { get; set; }

        public virtual Message Message { get; set; }

        public int? ChatId { get; set; }

        public virtual Chat Chat { get; set; }
    }
}
