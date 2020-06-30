using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Models
{
    public class ActiveKey
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string Key { get; set; }

        public DateTime Created { get; set; }

        public bool IsActive { get; set; }
    }
}
