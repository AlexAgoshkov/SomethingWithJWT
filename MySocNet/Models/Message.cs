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

       public int SendId { get; set; }

       public int ReciveId { get; set; }
       
       public string Text { get; set; }

       public int? ChatId { get; set; }

       public virtual Chat Chat { get; set; }
    }
}
