using Dapper;
using DapperSqlite.Models;
using DapperSqlite.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DapperSqlite.Services
{
    public class StatisticsActivedUserService : IStatisticsActivedUserService
    {
        private readonly SqLiteBase _sqLiteBase;
        private readonly SqliteCreateDatabase _sqliteCreateDatabase;

        public StatisticsActivedUserService()
        {
            _sqLiteBase = new SqLiteBase();
            _sqliteCreateDatabase = new SqliteCreateDatabase();

            if (!File.Exists(_sqLiteBase.DbFile))
            {
                _sqliteCreateDatabase.CreateDatabase();
            }
        }

        public void AddActivedUser(string userName)
        {
            using (var cnn = _sqLiteBase.SimpleDbConnection())
            {
                cnn.Open();
                cnn.Query<int>(
                    @"INSERT INTO NewUsers 
                    ( UserName ) VALUES 
                    ( @UserName );
                    select last_insert_rowid()", new { UserName = userName }).First();
            }
        }

        public int GetActivedUsersCount()
        {
            if (!File.Exists(_sqLiteBase.DbFile)) throw new DllNotFoundException("Database doesn't exists");

            using (var cnn = _sqLiteBase.SimpleDbConnection())
            {
                cnn.Open();
                int result = cnn.Query<ActivedUser>(
                    @"SELECT Id, UserName
                      FROM ActivedUsers").Count();
                return result;
            }
        }
    }
}
