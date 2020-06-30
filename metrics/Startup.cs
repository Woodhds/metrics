using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using metrics.Options;
using Base.Abstractions;
using Base.Contracts.Options;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;
using metrics.Data.Abstractions;
using metrics.Data.Common;
using metrics.Data.Common.Infrastructure.Confguraton;
using metrics.Data.Sql;
using metrics.Events;
using metrics.Handlers;
using metrics.Identity.Client;
using metrics.ML.Services.Extensions;
using metrics.Notification.SignalR.Extensions;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using metrics.Services.Utils;
using metrics.Web;
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
            provider.RegisterCommandConsumer<SetMessageTypeEvent, SetTypeEventHandler>();
            
            provider.RegisterCommand<CreateRepostGroup>();
            provider.RegisterCommand<SetMessageTypeEvent>();
        }

        protected override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.AddMetricsSignalR(CorsPolicy);
        }

        protected override void ConfigureApplicationServices(IServiceCollection services)
        {
            var signalROptions = Configuration.GetSection((nameof(SignalROptions))).Get<SignalROptions>();
            
            services.AddMetricsSignalR(signalROptions.Host);
            
            services.AddSingleton<IBaseHttpClient, BaseHttpClient>();
            services.AddScoped<ICompetitionsService, CompetitionsService>();
            services.AddScoped<IVkTokenAccessor, CacheTokenAccessor>();
            services.Configure<VkontakteOptions>(Configuration.GetSection(nameof(VkontakteOptions)));
            services.Configure<VkApiUrls>(Configuration.GetSection("VKApiUrls"));
            services.AddScoped<IVkClient, VkClient>();
            services.AddScoped<IVkUserService, VkUserService>();
            services.AddSingleton<IVkMessageService, VkMessageService>();
            services.AddSingleton<IRepostCacheAccessor, RepostCacheAccessor>();

            services.AddSingleton<IEntityConfiguration, RepostEntityConfiguration>();
            services.AddPredictClient(Configuration["ClientUrl"]);
            
            services.AddElastic(Configuration);
            
            services.AddIdentityClient(Configuration);
        }
    }
}