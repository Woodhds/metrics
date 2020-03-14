using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace metrics.Data.Sql
{
    public class DataContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly IEntityConfigurationProvider _entityConfigurationProvider;

        public DataContext(
            DbContextOptions options,
            IConfiguration configuration,
            IEntityConfigurationProvider entityConfigurationProvider
        ) : base(options)
        {
            _configuration = configuration;
            _entityConfigurationProvider = entityConfigurationProvider;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseNpgsql(_configuration.GetConnectionString("DataContext"));

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var config in _entityConfigurationProvider.GetConfigurations())
            {
                config.Configure(modelBuilder);
            }

            _entityConfigurationProvider.GetConfigurations();
            base.OnModelCreating(modelBuilder);
        }
    }
}