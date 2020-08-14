using DapperSqlite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DapperSqlite.Services.Interfaces
{
    public interface IStatisticsNewUserService
    {
        int GetNewUsersCount();
        void AddNewUser(string userName);
    }
}
