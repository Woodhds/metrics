using metrics.Cache.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace metrics.Cache
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCaching(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddStackExchangeRedisCache(x =>
            {
                x.ConfigurationOptions = ConfigurationOptions.Parse("192.168.99.100:30379,password=password");
                x.InstanceName = "RepostInstance";
            });

            serviceCollection.AddSingleton<ICachingSerializer, CachingSerializer>();
            serviceCollection.AddSingleton<ICachingService, CachingService>();
            
            return serviceCollection;
        }
    }
}