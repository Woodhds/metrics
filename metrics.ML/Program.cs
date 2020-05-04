using System.Threading.Tasks;
using Base.Abstractions;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Confguraton;
using metrics.Data.Sql;
using metrics.Data.Sql.Extensions;
using metrics.ML.Services;
using metrics.Services.Abstractions;
using metrics.Services.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace metrics.ML
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<VkMessageMLService>();
                    services.AddSingleton<IEntityConfiguration, RepostEntityConfiguration>();
                    services.AddDataContext<DataContext>(context.Configuration.GetConnectionString("DataContext"));
                    services.AddSingleton<IVkMessageService, VkMessageService>();
                    services.AddElastic(context.Configuration);
                });
    }
}