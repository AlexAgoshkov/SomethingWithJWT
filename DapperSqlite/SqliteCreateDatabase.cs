using Dapper;
using DapperSqlite.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DapperSqlite
{
    public class SqliteCreateDatabase : SqLiteBase
    {
        public void CreateDatabase()
        {
            using (var cnn = SimpleDbConnection())
            {
                cnn.Open();
                cnn.Execute(
                    @"create table NewUsers
                    (
                         Id integer primary key AUTOINCREMENT,
                         UserName varchar(100) not null
                    );
                    

                    create table ActivedUsers 
                    (
                         Id integer primary key AUTOINCREMENT,
                         UserName varchar(100) not null
                    );

                    create table SentMessages
                    (
                         Id integer primary key AUTOINCREMENT,
                         MessageId integer not null
                    ); 
                    ");
            }
        }
    }
}
