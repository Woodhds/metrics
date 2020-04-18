using System;
using Base.Contracts.Options;
using metrics.Broker;
using metrics.logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Web;

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
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<Startup>();
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
                    services.AddMessageBroker(context.Configuration);
                })
                .UseNLog();
    }
}