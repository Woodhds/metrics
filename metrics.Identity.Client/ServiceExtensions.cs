using Base.Contracts.Options;
using metrics.Authentication.Services.Abstract;
using metrics.Authentication.Services.Concrete;
using metrics.Identity.Client.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Identity.Client
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddIdentityClient(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            serviceCollection.AddHttpClient();
            serviceCollection.AddSingleton<IIdentityClient, IdentityClient>();
            serviceCollection.Configure<IdentityOptions>(configuration.GetSection(nameof(IdentityOptions)));
            serviceCollection.AddSingleton<ISystemTokenGenerationService, SystemTokenGenerationService>();
            serviceCollection.AddSingleton<IJsonWebTokenGenerationService, JsonWebTokenGenerationService>();
            return serviceCollection;
        }
    }
}