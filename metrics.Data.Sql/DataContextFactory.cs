using System.Collections.Generic;
using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace metrics.Data.Sql
{
    public class DataContextFactory : IDataContextFactory
    {
        private readonly DbContextOptions _options;
        private readonly IConfiguration _configuration;
        private readonly IEntityConfigurationProvider _entityConfigurationProvider;

        public DataContextFactory(
            DbContextOptions options,
            IConfiguration configuration,
            IEntityConfigurationProvider entityConfigurationProvider
        )
        {
            _options = options;
            _configuration = configuration;
            _entityConfigurationProvider = entityConfigurationProvider;
        }

        public DbContext Create()
        {
            return new DataContext(_options, _configuration, _entityConfigurationProvider);
        }
    }
}