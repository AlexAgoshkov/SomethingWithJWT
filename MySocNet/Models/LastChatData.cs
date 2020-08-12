using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Models
{
    public class LastChatData
    {
        [Key]
        public int Id { get; set; }
        
        public string UserName { get; set; }

        public string Text { get; set; }

        public int ChatId { get; set; }

        public virtual Chat Chat { get; set; }
    }
}
