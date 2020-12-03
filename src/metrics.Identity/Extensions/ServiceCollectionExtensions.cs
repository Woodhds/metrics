using metrics.Identity.Data;
using metrics.Identity.Data.Models;
using metrics.Identity.Data.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Identity.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMetricsIdentity(this IServiceCollection serviceCollection, string connectionString)
        {
            serviceCollection.AddDbContext<IdentityContext>(x =>
            {
                x.UseNpgsql(connectionString);
                x.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            
            serviceCollection.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddUserStore<UserStore>()
                .AddDefaultTokenProviders();

            serviceCollection.AddScoped<UserStore>();

            return serviceCollection;
        }
    }
}