using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class BaseContext : DbContext
    {
        public BaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasKey(z => z.Id);
            modelBuilder.Entity<Role>().HasKey(z => z.Id);
            modelBuilder.Entity<UserRole>().HasKey(z => new { z.RoleId, z.UserId });
        }
    }
}