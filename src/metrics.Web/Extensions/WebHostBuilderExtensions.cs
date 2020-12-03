using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace metrics.Web.Extensions
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder AddSharedConfiguration(this IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                var env = context.HostingEnvironment;
                var path = AppDomain.CurrentDomain.BaseDirectory;

                configurationBuilder.AddJsonFile(Path.Combine(path, "sharedsettings.json"), true);
                configurationBuilder.AddJsonFile(
                    Path.Combine(path, $"sharedsettings.{env.EnvironmentName}.json"), true);
            });

            return builder;
        }
    }
}