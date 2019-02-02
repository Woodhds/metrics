using System;
using System.IO;
using metrics.Services.Options;
using Microsoft.Extensions.Configuration;

namespace metrrics.services.tests
{
    public static class TestOptions
    {
        public static VKApiUrls GetUrls()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../metrics/appsettings.json"));
            var config = builder.Build();
            var urls = new VKApiUrls();
            config.GetSection("VkApiUrls").Bind(urls);
            return urls;
        }
    }
}