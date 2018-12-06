using DAL;
using Data.Entities;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.EF
{
    public sealed class DataContext : BaseContext
    {
        public DbSet<ParseMessage> ParseMessages { get; set; }

        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<VkUser>();
            modelBuilder.Entity<ParseMessage>().HasKey(c => new { c.Id, c.OwnerId });
        }
    }
}
