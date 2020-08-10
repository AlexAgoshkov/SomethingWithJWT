using Microsoft.AspNetCore.Builder;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySocNet.Logger;
using MySocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Extensions
{
    public static class MyLoggerExtensions
    { 
        public async static Task AddLogs(this ILogger logger, IApplicationBuilder app, LogData model)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<MyDbContext>();
                await context.LogData.AddAsync(model);
                await context.SaveChangesAsync();
            }
        }
    }
}
