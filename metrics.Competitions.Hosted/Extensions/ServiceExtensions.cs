using metrics.Authentication.Infrastructure;
using metrics.Competitions.Hosted.Services;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Competitions.Hosted.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddVkClientConsole(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<IUserTokenAccessor, ConsoleTokenAccessor>();
            services.AddSingleton<IAuthenticatedUserProvider, ConsoleUserProvider>();
            return services;
        }
    }
}