using Base.Contracts.Options;
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
                .ConfigureLogging((context, builder) =>
                {
                    ConfigSettingLayoutRenderer.DefaultConfiguration = context.Configuration;

                    builder.ClearProviders();

                    var logConfig = new NLogLoggingConfiguration(context.Configuration.GetSection("NLog"));
                    LogManager.Configuration = logConfig;

                    builder.AddProvider(new NLogLoggerProvider(NLogAspNetCoreOptions.Default, new LogFactory(logConfig)));
                    builder.SetMinimumLevel(LogLevel.Trace);
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