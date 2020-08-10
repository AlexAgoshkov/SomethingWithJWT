using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MySocNet.Logger
{
    public class ErrorDetails
    {
        public ErrorDetails(string message)
        {
            Message = message;
        }

        public string Message { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
