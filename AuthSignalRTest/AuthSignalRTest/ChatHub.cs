using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthSignalRTest
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task Send(string message, string userName)
        {
            await Clients.All.SendAsync("Receive", message, userName);
        }
        [Authorize(Roles = "admin")]
        public async Task Notify(string message, string userName)
        {
            await Clients.All.SendAsync("Receive", message, userName);
        }
    }
}
