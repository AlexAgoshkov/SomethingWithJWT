using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;
using MySocNet.Logger;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySocNet.Models
{
    public class MyDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Friend> Friends { get; set; }

        public DbSet<Authentication> Authentications { get; set; }

        public DbSet<ActiveKey> ActiveKeys { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<UserChat> UserChats { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<LogData> LogData { get; set; }

        public DbSet<UserMessage> UserMessages { get; set; }

        public DbSet<LastChatData> LastChatDatas { get; set; }

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserChat>()
                .HasKey(uc => new { uc.UserId, uc.ChatId });
            modelBuilder.Entity<UserChat>()
                .HasOne(uc => uc.User)
                .WithMany(c => c.UserChats)
                .HasForeignKey(uc => uc.UserId);
            modelBuilder.Entity<UserChat>()
                .HasOne(uc => uc.Chat)
                .WithMany(c => c.UserChats)
                .HasForeignKey(uc => uc.ChatId);

            modelBuilder.Entity<UserMessage>()
               .HasKey(uc => new { uc.ChatId, uc.UserId});
          

            modelBuilder.Entity<LogData>();
            base.OnModelCreating(modelBuilder);
        }

        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name
                                && level == LogLevel.Information)
                .AddProvider(new DbLoggerProvider());
        });
    }
}
