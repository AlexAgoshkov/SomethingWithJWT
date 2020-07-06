using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Friend
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }

        public virtual User User { get; set; }
    }
}
