using System;
using Consul;
using metrics.ServiceDiscovery.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.ServiceDiscovery
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceDiscovery(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConsulConfig>(configuration.GetSection(nameof(ConsulConfig)));

            services.AddSingleton<IConsulClient, ConsulClient>(provider => new ConsulClient(config =>
            {
                config.Address = new Uri(configuration["ConsulConfig:Address"]);
            }));

            return services;
        }
    }
}