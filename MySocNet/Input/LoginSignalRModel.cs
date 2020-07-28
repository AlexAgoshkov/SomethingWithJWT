using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Input
{
    public class LoginSignalRModel
    {
       
        public string ChatId { get; set; }

        
        public string AccessToken { get; set; }
    }
}
