using metrics.Data.Abstractions;
using metrics.Data.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Data.Common
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataContext<T>(this IServiceCollection services,
            string connectionString) where T : DbContext
        {
            void OptionsAction(DbContextOptionsBuilder builder)
            {
                builder.UseNpgsql(connectionString);
                builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }

            services.AddSingleton<IEntityConfigurationProvider, EntityConfigurationProvider>();
            services.AddSingleton<ITransactionScopeFactory, TransactionScopeFactory<T>>();
            services.AddDbContextFactory<T>(OptionsAction);

            return services;
        }
    }
}