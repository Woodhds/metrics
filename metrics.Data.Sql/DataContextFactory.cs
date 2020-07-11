using Microsoft.EntityFrameworkCore;

namespace metrics.Data.Sql
{
    public class DataContextFactory : IDataContextFactory
    {
        private readonly DbContextOptions _options;

        public DataContextFactory(DbContextOptions options)
        {
            _options = options;
        }

        public DataContext Create()
        {
            return new DataContext(_options);
        }
    }
}