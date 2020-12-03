using metrics.Cache.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace metrics.Cache
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCaching(this IServiceCollection serviceCollection, 
            IConfiguration configuration)
        {
            var options = new CacheOptions();
            configuration.GetSection(nameof(CacheOptions)).Bind(options);
            serviceCollection.AddStackExchangeRedisCache(x =>
            {
                x.ConfigurationOptions = ConfigurationOptions.Parse(options.Configuration);
                x.InstanceName = options.Instance;
            });

            serviceCollection.AddSingleton<ICachingSerializer, CachingSerializer>();
            serviceCollection.AddSingleton<ICachingService, CachingService>();
            
            return serviceCollection;
        }
    }
}