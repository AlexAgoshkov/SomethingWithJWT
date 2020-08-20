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
    public class StatisticsSentMessageService : IStatisticsSentMessageService
    {
        private readonly SqLiteBase _sqLiteBase;
        private readonly SqliteCreateDatabase _sqliteCreateDatabase;

        public StatisticsSentMessageService()
        {
            _sqLiteBase = new SqLiteBase();
            _sqliteCreateDatabase = new SqliteCreateDatabase();

            if (!File.Exists(_sqLiteBase.DbFile))
            {
                _sqliteCreateDatabase.CreateDatabase();
            }
        }

        public void AddNewMessage(int id)
        {
            using (var cnn = _sqLiteBase.SimpleDbConnection())
            {
                cnn.Open();
                cnn.Query<int>(
                    @"INSERT INTO SentMessages 
                    ( MessageId ) VALUES 
                    ( @MessageId );
                    select last_insert_rowid()", new { MessageId = id }).First();
            }
        }

        public int GetNewMessageCount()
        {
            if (!File.Exists(_sqLiteBase.DbFile)) throw new DllNotFoundException("Database doesn't exists");

            using (var cnn = _sqLiteBase.SimpleDbConnection())
            {
                cnn.Open();
                int result = cnn.Query<SentMessage>(
                    @"SELECT Id, MessageId
                      FROM SentMessages").Count();
                return result;
            }
        }
    }
}
