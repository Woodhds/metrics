using Base.Contracts.Options;
using metrics.Authentication;
using metrics.Authentication.Infrastructure;
using metrics.Authentication.Services.Abstract;
using metrics.Authentication.Services.Concrete;
using metrics.BackgroundJobs;
using metrics.Broker.Abstractions;
using metrics.Broker.Events.Events;
using metrics.Data.Abstractions;
using metrics.Data.Common;
using metrics.Data.Common.Infrastructure.Confguraton;
using metrics.Data.Sql;
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
            provider.RegisterCommandConsumer<CreateRepostGroup, RepostEventGroupCreatedHandler>();
            provider.RegisterConsumer<LoginEvent, LoginEventHandler>();
            provider.RegisterCommandConsumer<RepostCreated, RepostedEventHandler>();
            provider.RegisterCommandConsumer<ExecuteNextRepost, RepostUserEventHandler>();
            
            provider.RegisterCommand<ExecuteNextRepost>();
            provider.RegisterCommand<RepostCreated>();
        }

        protected override void ConfigureApplicationServices(IServiceCollection services)
        {
            services.AddScoped<IVkClient, VkClient>();
            
            services.AddScoped<ISchedulerJobService, SchedulerJobService>();
            services.AddSingleton<IUserRepostedService, UserRepostedService>();

            services.AddIdentityClient(Configuration);
            services.Configure<VkontakteOptions>(
                Configuration.GetSection(nameof(VkontakteOptions)));

            services.AddHangfire(Configuration["JobsHost"]);
        }

        protected override void ConfigureDataContext(IServiceCollection services)
        {
            services.AddSingleton<IEntityConfiguration, RepostEntityConfiguration>();
            services.AddDataContext<DataContext>(Configuration.GetConnectionString("DataContext"));
        }
    }
}