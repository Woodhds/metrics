using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace metrics.test
{
    public class TestDataContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("metrics");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VkUser>().HasData(new VkUser[] 
            {
                new VkUser { Id = 1, UserId = "123", FirstName = "Vasya", LastName = "Pupkin" },
                new VkUser { Id = 2, UserId = "445" } 
            });
        }
    }
}