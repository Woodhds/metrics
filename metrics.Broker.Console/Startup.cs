using Base.Contracts.Options;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Confguraton;
using metrics.Data.Sql;
using metrics.Data.Sql.Extensions;
using metrics.Identity.Client;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using metrics.Services.Utils;
using metrics.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker.Console
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void AddBrokerHandlers(IMessageHandlerProvider provider)
        {
            provider.RegisterConsumer<CreateRepostGroup, RepostEventGroupCreatedHandler>();
            provider.RegisterConsumer<LoginEvent, LoginEventHandler>();
            provider.RegisterConsumer<CreateRepost, RepostedEventHandler>();
            
            provider.RegisterCommand<CreateRepost>();
        }

        protected override void ConfigureApplicationServices(IServiceCollection services)
        {
            services.AddHostedService<RepostHostedService>();
            services.AddSingleton<IVkClient, VkClient>();
            services.Configure<VkApiUrls>(Configuration.GetSection(nameof(VkApiUrls)));

            services.AddSingleton<IBaseHttpClient, BaseHttpClient>();
            services.AddSingleton<IVkTokenAccessor, CacheTokenAccessor>();
            services.AddSingleton<IRepostCacheAccessor, RepostCacheAccessor>();

            services.AddIdentityClient(Configuration);
            services.Configure<VkontakteOptions>(
                Configuration.GetSection(nameof(VkontakteOptions)));
        }

        protected override void ConfigureDataContext(IServiceCollection services)
        {
            services.AddSingleton<IEntityConfiguration, RepostEntityConfiguration>();
            services.AddDataContext<DataContext>(Configuration.GetConnectionString("DataContext"));
        }
    }
}