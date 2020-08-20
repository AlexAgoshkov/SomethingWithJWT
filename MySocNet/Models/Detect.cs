using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Models
{
    public class Detect
    {
        [Key]
        public int Id { get; set; }

        public string DeviceType { get; set; }

        public string Os { get; set; }

        public int? UserId { get; set; }
        
        public string Browser { get; set; }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Detect p = (Detect)obj;
                return (DeviceType == p.DeviceType) && (Os == p.Os) && (Browser == p.Browser);
            }   
        }
    }
}
