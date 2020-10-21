using Base.Contracts.Options;
using Elastic.Client;
using Hangfire;
using metrics.BackgroundJobs;
using metrics.Broker.Abstractions;
using metrics.Competitions.Abstractions;
using metrics.Competitions.Hosted.Services;
using metrics.Data.Abstractions;
using metrics.Data.Common;
using metrics.Data.Common.Infrastructure.Configuration;
using metrics.Data.Sql;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using metrics.Services.Extensions;
using metrics.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace metrics.Competitions.Hosted
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
        }
        protected override void AddBrokerHandlers(IMessageHandlerProvider provider)
        {
            
        }

        protected override void ConfigureApplicationServices(IServiceCollection services)
        {
            services.AddVkClient(Configuration);
            services.AddVkClientConsole(Configuration);
            services.AddSingleton<IElasticClientFactory, ElasticClientFactory>();
            services.AddSingleton<ICompetitionsService, CompetitionsService>();
            services.AddSingleton<ICompetitionsService, VkUserCompetitionService>();
            services.AddSingleton<IUserService, UserService>();
            services.Configure<CompetitionOptions>(Configuration.GetSection(nameof(CompetitionOptions)));
            services.Configure<ElasticOptions>(Configuration.GetSection(nameof(ElasticOptions)));
            services.AddSingleton<ICompetitionService, CompetitionService>();
            services.AddHostedService<CompetitionHostedService>();
            services.AddHangfire(Configuration["JobsHost"]);
        }

        protected override void ConfigureDataContext(IServiceCollection services)
        {
            services.AddDataContext<DataContext>(Configuration.GetConnectionString("DataContext"));
            services.AddSingleton<IEntityConfiguration, RepostEntityConfiguration>();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifeTime)
        {
            base.Configure(app, env, lifeTime);
            app.UseHangfireServer(new BackgroundJobServerOptions {Queues = new[] {"competition"}});
        }
    }
}