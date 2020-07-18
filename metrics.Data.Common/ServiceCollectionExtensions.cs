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
            services.AddSingleton<IEntityConfigurationProvider, EntityConfigurationProvider>();
            services.AddSingleton<ITransactionScopeFactory, TransactionScopeFactory>();
            services.AddSingleton<IDataContextFactory, DataContextFactory>();
            services.AddTransient<DbContext, T>();
            services.AddDbContextPool<T>(x =>
            {
                x.UseNpgsql(connectionString);
                x.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            return services;
        }
    }
}