using Base.Contracts.Options;
using metrics.Authentication.Infrastructure;
using metrics.core.DistributedLock;
using metrics.Services.Abstractions;
using metrics.Services.Abstractions.VK;
using metrics.Services.Concrete;
using metrics.Services.Concrete.VK;
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
            services.AddSingleton<IVkService, VkService>();
            services.AddHttpClient<IVkClient, VkClient>()
                .AddHttpMessageHandler<VkClientHttpHandler>();
            
            services.AddSingleton<IVkGroupService, VkGroupService>();
            services.AddSingleton<IVkLikeService, VkLikeService>();
            services.AddSingleton<IVkUserService, VkUserService>();
            services.AddSingleton<IVkWallService, VkWallService>();
            services.AddTransient<VkClientHttpHandler>();
            
            services.AddSingleton<IDistributedLock, DistributedLock>(x =>
                new DistributedLock(ConnectionMultiplexer.Connect(configuration[RedisConnectionStringLock]).GetDatabase()));
            return services;
        }
    }
}