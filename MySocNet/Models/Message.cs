using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MySocNet.Models
{
    public class Message
    {
       [Key]
       public int Id { get; set; }

       public string Text { get; set; }

        public int? ImageId { get; set; }

        public virtual Image MessageImage { get; set; }

        public int? ChatId { get; set; }

       public int? SenderId { get; set; }

       public DateTime Time { get; set; }

       public bool IsRead { get; set; }

       [JsonIgnore]
       public virtual User Sender { get; set; }

       [JsonIgnore]
       public virtual Chat Chat { get; set; }
    }
}
