using System.IO;
using metrics.Web.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace metrics.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile("ocelot.json");
                    builder.AddJsonFile(Path.Combine("secrets", "ocelot.json"), optional: true, reloadOnChange: true);
                })
                .AddSharedConfiguration()
                .UseStartup<Startup>();
    }
}