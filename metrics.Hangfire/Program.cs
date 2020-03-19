using Hangfire;
using Hangfire.PostgreSql;
using metrics.Data.Sql;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using metrics.Services.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace metrics.Hangfire
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IVkClient, VkClient>();
                    services.AddHttpClient();
                    services.AddSingleton<IBaseHttpClient, BaseHttpClient>();
                    services.AddSingleton<IVkTokenAccessor, CacheTokenAccessor>();
                    services.AddDbContextPool<DataContext>(x =>
                    {
                        x.EnableSensitiveDataLogging();
                    });
                    
                    services.AddHangfireServer();
                    services.AddHangfire((provider, configuration) =>
                    {
                        configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
                        configuration.UseSimpleAssemblyNameTypeSerializer();
                        configuration.UseRecommendedSerializerSettings();
                        configuration.UseStorage(
                            new PostgreSqlStorage(hostContext.Configuration.GetConnectionString("DataContext")));
                    });
                });
    }
}