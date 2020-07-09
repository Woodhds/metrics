using metrics.Authentication.Infrastructure;
using metrics.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Services.Utils
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddVkClientConsole(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<IBaseHttpClient, BaseHttpClient>();
            services.AddSingleton<IVkTokenAccessor, ConsoleTokenAccessor>();
            services.AddSingleton<IAuthenticatedUserProvider, ConsoleUserProvider>();
            return services;
        }
    }
}