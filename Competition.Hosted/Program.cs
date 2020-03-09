using System;
using Base.Contracts.Options;
using metrics.logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Competition.Hosted
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                
                CreateHostBuilder(args).Build().Run();
                Console.WriteLine($"Started at: {DateTime.Now}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
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
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddMetricsLogging(context.Configuration);
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<ElasticOptions>(context.Configuration.GetSection(nameof(ElasticOptions)));
                    services.Configure<VkApiUrls>(context.Configuration.GetSection(nameof(VkApiUrls)));
                    services.Configure<TokenOptions>(context.Configuration.GetSection("Token"));
                    services.Configure<CompetitionOptions>(
                        context.Configuration.GetSection(nameof(CompetitionOptions)));
                    services.AddHostedService<CompetitionService>();
                })
                .UseNLog();
    }
}