using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Logger
{
    public class LogData
    {
        [Key]
        public int Id { get; set; }

        public string Category { get; set; }

        public string Message { get; set; }

        public string User { get; set; }

        public int UserId { get; set; }

        public string Chat { get; set; }

        public int ChatId { get; set; }
    }
}
