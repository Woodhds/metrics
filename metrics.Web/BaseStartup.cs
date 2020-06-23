using metrics.Authentication.Options;
using metrics.Broker;
using metrics.Broker.Abstractions;
using metrics.Cache;
using metrics.logging;
using metrics.ServiceDiscovery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace metrics.Web
{
    public abstract class BaseStartup
    {
        protected readonly IConfiguration Configuration;

        protected BaseStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureBase(services);
            ConfigureMvc(services);
            ConfigureCaching(services);
            ConfigureDataContext(services);
            ConfigureAuthentication(services);
            ConfigureLogging(services);
            ConfigureApplicationServices(services);

            ConfigureMessageBroker(services);
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifeTime)
        {
            app.UseServiceDiscovery(lifeTime);
        }

        protected virtual void ConfigureMessageBroker(IServiceCollection services)
        {
            services.AddMessageBroker(Configuration, AddBrokerHandlers);
        }

        protected virtual void ConfigureAuthentication(IServiceCollection services)
        {
            services.Configure<JwtOptions>(Configuration.GetSection(nameof(JwtOptions)));
        }

        protected virtual void ConfigureMvc(IServiceCollection services)
        {
           
        }

        protected virtual void ConfigureBase(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddServiceDiscovery(Configuration);
        }

        protected virtual void ConfigureCaching(IServiceCollection services)
        {
            services.AddCaching(Configuration);
        }

        protected virtual void ConfigureLogging(IServiceCollection services)
        {
            services.AddLogging(c => c.AddMetricsLogging(Configuration));
        }

        protected abstract void AddBrokerHandlers(IMessageHandlerProvider provider);
        protected abstract void ConfigureApplicationServices(IServiceCollection services);
        protected abstract void ConfigureDataContext(IServiceCollection services);
    }
}