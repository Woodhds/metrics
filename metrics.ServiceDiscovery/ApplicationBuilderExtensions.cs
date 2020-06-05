using System;
using System.Linq;
using Consul;
using metrics.ServiceDiscovery.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace metrics.ServiceDiscovery
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseServiceDiscovery(this IApplicationBuilder app,
            IHostApplicationLifetime lifeTime)
        {
            var consulClient = app.ApplicationServices
                .GetRequiredService<IConsulClient>();

            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>();
            var address = addresses.Addresses.FirstOrDefault();
            if (string.IsNullOrEmpty(address))
                return app;
            
            var uri = new Uri(address);
            var registration = new AgentServiceRegistration
            {
                ID = $"{AppDomain.CurrentDomain.FriendlyName}-{uri.Port}",
                Name = AppDomain.CurrentDomain.FriendlyName,
                Address = $"{uri.Scheme}://{uri.Host}",
                Port = uri.Port,
            };
            
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            consulClient.Agent.ServiceRegister(registration).Wait();

            lifeTime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });
            
            return app;
        }
    }
}