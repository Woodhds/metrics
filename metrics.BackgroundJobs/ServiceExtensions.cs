using Hangfire;
using Hangfire.Redis;
using metrics.BackgroundJobs.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace metrics.BackgroundJobs
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddHangfire(this IServiceCollection services, string host)
        {
            services.AddHangfireServer(options =>
            {
            });

            services.AddHangfire(configuration =>
            {
                configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
                configuration.UseSimpleAssemblyNameTypeSerializer();
                configuration.UseRecommendedSerializerSettings();
                configuration.UseRedisStorage(ConnectionMultiplexer.Connect(host), new RedisStorageOptions());
            });

            services.AddSingleton<IBackgroundJobService, BackgroundJobService>();
            
            return services;
        }
    }
}