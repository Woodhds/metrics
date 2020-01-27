using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace metrics
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://localhost:1745", "https://localhost:1746");
                    webBuilder.UseStartup<Startup>();
                });
    }
}