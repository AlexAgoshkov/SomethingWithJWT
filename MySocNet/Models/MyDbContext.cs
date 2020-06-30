using Microsoft.EntityFrameworkCore;
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

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasData(new User
            {
                UserId = 1,
                UserName = "ggg",
                FirstName = "Larry",
                SurName = "Richi",
                Email = "example@mail.hock",
                Password = HashService.Hash("666"),
                UserRole = "Admin",
            }); 

            modelBuilder.Entity<Friend>().HasData(new Friend 
            {
                FriendID = 1,                               
                UserAddedId = 1,                                      
                UserID = 1 
            });
        }
    }
}
