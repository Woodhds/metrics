using Base.Contracts;
using Base.Contracts.Options;
using Hangfire;
using metrics.BackgroundJobs;
using metrics.Broker.Abstractions;
using metrics.Broker.Console.Events.Handlers;
using metrics.Broker.Console.Services;
using metrics.Broker.Events.Events;
using metrics.Data.Abstractions;
using metrics.Data.Common;
using metrics.Data.Common.Infrastructure.Configuration;
using metrics.Data.Sql;
using metrics.Identity.Client;
using metrics.Identity.Client.Abstractions;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using metrics.Services.Extensions;
using metrics.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace metrics.Broker.Console
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void AddBrokerHandlers(IMessageHandlerProvider provider)
        {
            provider.RegisterConsumer<ILoginEvent, LoginEventHandler>();
            provider.RegisterConsumer<IUserTokenRemoved, UserTokenRemovedHandler>();
            provider.RegisterConsumer<IUserTokenChanged, UserTokenChangedHandler>();
            provider.RegisterCommandConsumer<ICreateRepostGroup, RepostEventGroupCreatedHandler>();
            provider.RegisterCommandConsumer<IRepostCreated, RepostedEventHandler>();
            provider.RegisterCommandConsumer<IExecuteNextRepost, RepostUserEventHandler>();
            
            provider.RegisterCommand<IExecuteNextRepost>();
            provider.RegisterCommand<IRepostCreated>();
        }

        protected override void ConfigureApplicationServices(IServiceCollection services)
        {
            services.AddVkClient(Configuration);
            //services.AddHostedService<StartupService>();
            services.AddSingleton<IRandomLikeService, RandomLikeService>();
            
            services.AddSingleton<ISchedulerJobService, SchedulerJobService>();
            services.AddSingleton<IUserRepostedService, UserRepostedService>();
            services.AddSingleton<IUserTokenKeyProvider, UserTokenKeyProvider>();

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

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifeTime)
        {
            base.Configure(app, env, lifeTime);
            app.UseHangfireDashboard();
        }
    }
}