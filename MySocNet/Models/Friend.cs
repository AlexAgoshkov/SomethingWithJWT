using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Models
{
    public class Friend
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserAddedId { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
