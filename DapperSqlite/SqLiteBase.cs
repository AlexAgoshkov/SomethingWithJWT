using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DapperSqlite
{
    public class SqLiteBase
    {
        public string DbFile
        {
            get { return Environment.CurrentDirectory + "\\Statistics.sqlite"; }
        }

        public  SqliteConnection SimpleDbConnection()
        {
            return new SqliteConnection("Data Source=" + DbFile);
        }
    }
}
