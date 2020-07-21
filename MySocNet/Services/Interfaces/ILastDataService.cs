using MySocNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Services.Interfaces
{
    public interface ILastDataService
    {
        Task AddLastChatData(LastChatData lastChatData);

        Task<IList<LastChatData>> GetLastData(int userId);
    }
}
