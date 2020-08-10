using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using MySocNet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Logger
{
    public class MyLoggerProvider : ILoggerProvider
    {
        private readonly IApplicationBuilder _app;
    
        public MyLoggerProvider(IApplicationBuilder app)
        {
            _app = app;
        }

        public ILogger CreateLogger(string categoryName)
        {
           return new Logger(_app, categoryName);
        }

        public void Dispose()
        {

        }
    }
}
