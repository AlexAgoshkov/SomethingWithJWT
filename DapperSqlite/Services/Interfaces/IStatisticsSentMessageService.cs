using DapperSqlite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DapperSqlite.Services.Interfaces
{
    public interface IStatisticsSentMessageService
    {
        int GetNewMessageCount();
        void AddNewMessage(int id);
    }
}
