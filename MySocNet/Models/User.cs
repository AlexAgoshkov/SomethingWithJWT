using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
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
        public int Id { get; set; }

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
        [JsonIgnore]
        public string Password { get; set; }

        public string UserRole { get; set; }

        public virtual ICollection<Detect> Detects { get; set; }

        public int? ImageId { get; set; }

        public virtual Image ProfileImage { get; set; }
        
        public int? ActiveKeyId { get; set; }
        
        public virtual ActiveKey ActiveKey { get; set; }

        public int? AuthenticationId { get; set; } 
        
        public virtual Authentication Authentication { get; set; }
        [JsonIgnore]
        public virtual ICollection<Friend> Friends { get; set; }
        [JsonIgnore]
        public virtual ICollection<Chat> Chats { get; set; }
        [JsonIgnore]
        public virtual ICollection<UserChat> UserChats { get; set; }
        [JsonIgnore]
        public virtual ICollection<UserChatRead> UserMessages { get; set; }

        public User()
        {
            Friends = new List<Friend>();
        }
    }
}
