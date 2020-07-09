using System;
using Base.Abstractions;
using Base.Contracts.Options;
using metrics.Broker.Abstractions;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using metrics.Services.Utils;
using metrics.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Competition.Hosted
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
            services.AddHttpClient<IVkClient, VkClient>((provider, client) =>
            {
                client.BaseAddress = new Uri(VkApiUrls.Domain);
            });
            services.AddVkClientConsole();
            services.AddSingleton<IElasticClientFactory, ElasticClientFactory>();
            services.AddTransient<ICompetitionsService, CompetitionsService>();
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