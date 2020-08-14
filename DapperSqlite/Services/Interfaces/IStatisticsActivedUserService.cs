using DapperSqlite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DapperSqlite.Services.Interfaces
{
    public interface IStatisticsActivedUserService
    {
        int GetActivedUsersCount();
        void AddActivedUser(string userName);
    }
}
