using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySocNet.Models;
using MySocNet.Services.Interfaces;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Logger
{
    public class MyLogger : IMyLogger
    {
        private readonly IRepository<LogData> _logDataRepository;

        public MyLogger(IRepository<LogData> logDataRepository)
        {
            _logDataRepository = logDataRepository;
        }
        public async Task AddLog(LogData logData)
        {
            await _logDataRepository.AddAsync(logData);
        }
    }
}
