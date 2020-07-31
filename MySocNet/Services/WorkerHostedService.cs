using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySocNet.Models;
using MySocNet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MySocNet.Services
{
    public class WorkerHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public WorkerHostedService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            while (!stopToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(30));

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var myScopedService = scope.ServiceProvider.GetRequiredService<IRepository<User>>();
                    var users = await myScopedService
                        .GetWhere(x => !x.ActiveKey.IsActive && 59 - x.ActiveKey.Created.Minute > 2)
                        .Include(x => x.ActiveKey).ToListAsync();
                    if (users != null)
                    {
                       await myScopedService.RemoveRangeAsync(users);
                    }
                }
            }
        }
    }
}
