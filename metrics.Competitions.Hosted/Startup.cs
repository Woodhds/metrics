using Base.Contracts.Options;
using Elastic.Client;
using metrics.Broker.Abstractions;
using metrics.Competitions.Abstractions;
using metrics.Competitions.Hosted.Extensions;
using metrics.Competitions.Hosted.Services;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using metrics.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddSingleton<IVkClient, VkClient>();
            services.AddVkClientConsole();
            services.AddSingleton<IElasticClientFactory, ElasticClientFactory>();
            services.AddSingleton<ICompetitionsService, CompetitionsService>();
            services.AddSingleton<ICompetitionsService, VkUserCompetitionService>();
            services.AddSingleton<IVkUserService, VkUserService>();
            services.Configure<TokenOptions>(Configuration.GetSection("Token"));
            services.Configure<VkontakteOptions>(Configuration.GetSection(nameof(VkontakteOptions)));
            services.Configure<CompetitionOptions>(Configuration.GetSection(nameof(CompetitionOptions)));
            services.Configure<ElasticOptions>(Configuration.GetSection(nameof(ElasticOptions)));
            services.AddHostedService<CompetitionService>();
        }

        protected override void ConfigureDataContext(IServiceCollection services)
        {
        }
    }
}