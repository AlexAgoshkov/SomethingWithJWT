using Microsoft.AspNetCore.Identity;
using MySocNet.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MySocNet.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(25)]
        [Column(TypeName = "varchar(25)")]
        public string UserName { get; set; }

       
        [MaxLength(25)]
        [Column(TypeName = "varchar(25)")]
        public string FirstName { get; set;}

        [MaxLength(25)]
        [Column(TypeName = "varchar(25)")]
        public string SurName { get; set; }

        
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Email { get; set; }

        [Required]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(max)")]
        public string Password { get; set; }

        public string UserRole { get; set; }

        [JsonIgnore]
        public int? AuthenticationId { get; set; }

        [JsonIgnore]
        public virtual Authentication Authentication { get; set; }

        public virtual ICollection<Friend> Friends { get; set; }

        public User()
        {
            Friends = new List<Friend>();
        }
    }
}
