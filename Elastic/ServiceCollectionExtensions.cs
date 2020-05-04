using Base.Contracts.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Base.Abstractions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElastic(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IElasticClientFactory, ElasticClientFactory>();
            services.Configure<ElasticOptions>(configuration.GetSection("ElasticOptions"));
            return services;
        }
    }
}