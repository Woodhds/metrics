using metrics.Cache.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Cache
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCaching(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddStackExchangeRedisCache(x =>
            {
                x.Configuration = "localhost";
                x.InstanceName = "RepostInstance";
            });

            serviceCollection.AddSingleton<ICachingSerializer, CachingSerializer>();
            serviceCollection.AddSingleton<ICachingService, CachingService>();
            
            return serviceCollection;
        }
    }
}