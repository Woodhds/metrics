using Data.Entities;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace Data.EF
{
    public sealed class DataContext : BaseContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>();
            modelBuilder.Entity<Order>();
        }
    }
}
