using Microsoft.Extensions.Logging;
using MySocNet.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services.Interfaces
{
    public interface IMyLogger
    {
        Task AddLog(LogData logData);
    }
}
