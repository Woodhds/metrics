using metrics.Data.Abstractions.Extensions;
using Microsoft.EntityFrameworkCore;

namespace metrics.Data.Abstractions
{
    public abstract class BaseDataContext : DbContext
    {
        protected BaseDataContext(DbContextOptions options) : base(options) {}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureEntities(modelBuilder);

            modelBuilder.SetDateTimeConverter();
            
            base.OnModelCreating(modelBuilder);
        }

        protected abstract void ConfigureEntities(ModelBuilder builder);
    }
}