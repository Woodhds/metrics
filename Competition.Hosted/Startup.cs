using Base.Abstractions;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using metrics.Services.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Competition.Hosted
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IVkClient, VkClient>();
            services.AddVkClientConsole();
            services.AddSingleton<IElasticClientFactory, ElasticClientFactory>();
            services.AddTransient<ICompetitionsService, CompetitionsService>();
            services.AddSingleton<ICompetitionsService, VkUserCompetitionService>();
            services.AddSingleton<IVkUserService, VkUserService>();
        }

        public void Configure(IWebHostEnvironment env)
        {
            
        }
    }
}