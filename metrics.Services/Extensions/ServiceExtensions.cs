using Base.Contracts.Options;
using metrics.Authentication.Infrastructure;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Services.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddVkClientConsole(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();
            services.AddSingleton<IUserTokenAccessor, ConsoleTokenAccessor>();
            services.AddSingleton<IAuthenticatedUserProvider, ConsoleUserProvider>();
            services.Configure<TokenOptions>(configuration.GetSection("Token"));
            services.Configure<VkontakteOptions>(configuration.GetSection(nameof(VkontakteOptions)));
            return services;
        }
    }
}