using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace metrics.Data.Sql
{
    public class DataContext : BaseDataContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void ConfigureEntities(ModelBuilder builder)
        {
            foreach (var config in this.GetService<IEntityConfigurationProvider>().GetConfigurations())
            {
                config.Configure(builder);
            }
        }
    }
}