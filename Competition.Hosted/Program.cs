using System;
using metrics.Web.Extensions;
using Microsoft.AspNetCore.Hosting;
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
                    builder.AddSharedConfiguration();
                    builder.UseStartup<Startup>();
                })
                .UseNLog();
    }
}