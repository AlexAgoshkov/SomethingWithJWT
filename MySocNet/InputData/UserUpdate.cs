using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.InputData
{
    public class UserUpdate
    {
        public string FirstName { get; set; }

        public string SurName { get; set; }

        public string Email { get; set; }
    }
}
