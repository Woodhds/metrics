using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace metrics.Data.Sql
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var config in this.GetService<IEntityConfigurationProvider>().GetConfigurations())
            {
                config.Configure(modelBuilder);
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}