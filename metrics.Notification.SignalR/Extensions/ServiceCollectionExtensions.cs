using Microsoft.Extensions.DependencyInjection;

namespace metrics.Notification.SignalR.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMetricsSignalR(this IServiceCollection services, string connectionString)
        {
            services.AddSignalR()
                .AddStackExchangeRedis(connectionString);

            return services;
        }
    }
}