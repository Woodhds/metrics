using metrics.Notification.SignalR.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace metrics.Notification.SignalR.Extensions
{
    public static class AppExtensions
    {
        public static IEndpointRouteBuilder AddMetricsSignalR(this IEndpointRouteBuilder builder, string corsPolicy)
        {
            builder.MapHub<NotificationHub>("/notifications").RequireCors(corsPolicy);
            return builder;
        }
    }
}