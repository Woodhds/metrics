using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using metrics.Options;
using Base.Contracts.Options;
using Elastic;
using Elastic.Client;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;
using metrics.Data.Abstractions;
using metrics.Data.Common;
using metrics.Data.Common.Infrastructure.Configuration;
using metrics.Data.Sql;
using metrics.Events;
using metrics.Handlers;
using metrics.Identity.Client;
using metrics.Identity.Client.Abstractions;
using metrics.Infrastructure;
using metrics.ML.Services.Extensions;
using metrics.Notification.SignalR.Extensions;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using metrics.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace metrics
{
    public class Startup: BaseWebStartup
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void ConfigureDataContext(IServiceCollection services)
        {
            services.AddDataContext<DataContext>(Configuration.GetConnectionString("DataContext"));
        }

        protected override void AddBrokerHandlers(IMessageHandlerProvider provider)
        {
            provider.RegisterConsumer<NotifyUserEvent, NotifyUserEventHandler>();
            provider.RegisterCommandConsumer<ISetMessageTypeEvent, SetTypeEventHandler>();
            
            provider.RegisterCommand<ICreateRepostGroup>();
            provider.RegisterCommand<IRepostCreated>();
            provider.RegisterCommand<ISetMessageTypeEvent>();
        }

        protected override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.AddMetricsSignalR(CorsPolicy);
        }

        protected override void ConfigureApplicationServices(IServiceCollection services)
        {
            var signalROptions = Configuration.GetSection(nameof(SignalROptions)).Get<SignalROptions>();
            
            services.AddMetricsSignalR(signalROptions.Host);
            
            services.Configure<VkontakteOptions>(Configuration.GetSection(nameof(VkontakteOptions)));
            services.AddScoped<IVkClient, VkClient>();
            services.AddScoped<IVkUserService, VkUserService>();
            services.AddSingleton<IVkMessageService, VkMessageService>();
            services.AddSingleton<IUserRepostedService, UserRepostedService>();
            services.AddSingleton<IUserTokenKeyProvider, UserTokenKeyProvider>();

            services.AddSingleton<IEntityConfiguration, RepostEntityConfiguration>();
            services.AddPredictClient(Configuration["ClientUrl"]);
            
            services.AddElastic(Configuration);
            
            services.AddIdentityClient(Configuration);
        }

        protected override void ConfigureManualMiddleware(IApplicationBuilder app)
        {
            app.UseMiddleware<IdentityUserMiddleware>();
        }
    }
}