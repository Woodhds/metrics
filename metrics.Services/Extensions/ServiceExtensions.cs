using Base.Contracts.Options;
using metrics.Authentication.Infrastructure;
using metrics.core.DistributedLock;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace metrics.Services.Extensions
{
    public static class ServiceExtensions
    {

        private const string RedisConnectionStringLock = nameof(RedisConnectionStringLock);
        
        public static IServiceCollection AddVkClientConsole(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpClient();
            services.AddSingleton<IUserTokenAccessor, ConsoleTokenAccessor>();
            services.AddSingleton<IAuthenticatedUserProvider, ConsoleUserProvider>();
            services.Configure<TokenOptions>(configuration.GetSection("Token"));
            services.Configure<VkontakteOptions>(configuration.GetSection(nameof(VkontakteOptions)));
            return services;
        }

        public static IServiceCollection AddVkClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IVkClient, VkClient>();
           // services.AddHttpClient<IVkClient, VkClient>();
            services.AddSingleton<IDistributedLock, DistributedLock>(x =>
                new DistributedLock(ConnectionMultiplexer.Connect(configuration[RedisConnectionStringLock]).GetDatabase()));
            return services;
        }
    }
}