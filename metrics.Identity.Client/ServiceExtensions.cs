using Base.Contracts.Options;
using metrics.Authentication;
using metrics.Authentication.Infrastructure;
using metrics.Authentication.Services.Abstract;
using metrics.Authentication.Services.Concrete;
using metrics.Identity.Client.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Identity.Client
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddIdentityClient(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpClient();
            services.AddSingleton<IIdentityClient, IdentityClient>();
            services.Configure<IdentityOptions>(configuration.GetSection(nameof(IdentityOptions)));
            services.AddSingleton<ISystemTokenGenerationService, SystemTokenGenerationService>();
            services.AddSingleton<IJsonWebTokenGenerationService, JsonWebTokenGenerationService>();
            services.AddScoped<IUserTokenAccessor, CachedUserTokenAccessor>();
            services.AddScoped<IUserStore, UserStore>();
            services.AddScoped<IAuthenticatedUserProvider, AuthenticatedUserProvider>();
            services.AddScoped<ISecurityUserManager, ApplicationUserManager>();
            return services;
        }
    }
}