using Base.Abstractions;
using metrics.Services.Abstract;
using metrics.Services.Concrete;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Competition.Hosted
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IVkClient, VkClient>();
            services.AddHttpClient();
            services.AddSingleton<IBaseHttpClient, BaseHttpClient>();
            services.AddSingleton<IVkTokenAccessor, ConsoleTokenAccessor>();
            services.AddSingleton<IElasticClientProvider, ElasticClientProvider>();
            services.AddSingleton<ICompetitionsService, CompetitionsService>();
        }

        public void Configure(IWebHostEnvironment env)
        {
            
        }
    }
}