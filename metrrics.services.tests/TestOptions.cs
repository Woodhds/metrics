using System;
using System.IO;
using metrics.Services.Abstract;
using metrics.Services.Concrete;
using metrics.Services.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace metrics.services.tests
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

        public static IVkClient GetClient()
        {
            var urls = GetUrls();
            return new VkClient(new TestHttpClientFactory(), new HttpContextAccessor { HttpContext = new TestHttpContext() },
                new OptionsWrapper<VKApiUrls>(urls), new NullLogger<VkClient>());
        }
    }
}