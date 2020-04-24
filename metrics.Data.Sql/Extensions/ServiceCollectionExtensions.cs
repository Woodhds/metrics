using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Data.Sql.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataContext<T>(this IServiceCollection services,
            string connectionString) where T: DbContext
        {
            services.AddSingleton<IEntityConfigurationProvider, EntityConfigurationProvider>();
            services.AddSingleton<ITransactionScopeFactory, TransactionScopeFactory>();
            services.AddSingleton<IDataContextFactory, DataContextFactory>();
            services.AddScoped<DbContext, T>();
            services.AddDbContextPool<T>(x =>
            {
                x.EnableSensitiveDataLogging();
                x.UseNpgsql(connectionString);
                x.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            return services;
        }
    }
}