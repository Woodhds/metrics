using System;
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
            services.AddGrpcClient<IdentityTokenService.IdentityTokenServiceClient>((serviceProvider, options) =>
                {
                    options.Address = new Uri(configuration["IdentityOptions:ServerUrl"]);
                })
                .AddInterceptor<IdentityClientAuthInterceptor>();
            services.AddSingleton<IdentityClientAuthInterceptor>();
            services.Configure<IdentityOptions>(configuration.GetSection(nameof(IdentityOptions)));
            services.AddSingleton<ISystemTokenGenerationService, SystemTokenGenerationService>();
            services.AddSingleton<IJsonWebTokenGenerationService, JsonWebTokenGenerationService>();
            services.AddSingleton<IUserTokenAccessor, CachedUserTokenAccessor>();
            services.AddSingleton<IUserStore, UserStore>();
            services.AddSingleton<IAuthenticatedUserProvider, AuthenticatedUserProvider>();
            services.AddSingleton<ISecurityUserManager, ApplicationUserManager>();
            return services;
        }
    }
}