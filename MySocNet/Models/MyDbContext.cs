using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySocNet.Logger;
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


            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                UserName = "ggg",
                FirstName = "Larry",
                SurName = "Richi",
                Email = "example@mail.hock",
                Password = HashService.Hash("666"),
                UserRole = "Admin",
                AuthenticationId = 1
            });

            modelBuilder.Entity<Friend>().HasData(new Friend
            {
                Id = 1,
                UserAddedId = 1,
                UserId = 1
            });

            modelBuilder.Entity<LogData>();
        }

        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name
                                && level == LogLevel.Information)
                .AddProvider(new DbLoggerProvider());
        });
    }
}
