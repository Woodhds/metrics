using Base.Contracts.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Competition.Hosted
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile("appsettings.json", true, true);
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<Startup>();
                    builder.UseUrls("http://localhost:5274", "https://localhost:5275");
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<ElasticOptions>(context.Configuration.GetSection("ElasticOptions"));
                    services.Configure<VKApiUrls>(context.Configuration.GetSection("VkApiUrls"));
                    services.Configure<TokenOptions>(context.Configuration.GetSection("Token"));
                    services.AddHostedService<CompetitionService>();
                });
    }
}