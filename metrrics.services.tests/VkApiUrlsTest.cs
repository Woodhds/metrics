using System;
using System.IO;
using System.Reflection;
using metrics.Services.Options;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace metrrics.services.tests
{
    [TestClass]
    public class VkApiUrlsTest
    {
        [TestMethod]
        public void UrlsTest()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../metrics/appsettings.json"));
            var config = builder.Build();
            var urls = new VKApiUrls();
            config.GetSection("VkApiUrls").Bind(urls);
            Assert.AreEqual(urls.Like, "likes.add");
            Assert.AreEqual(urls.Wall, "wall.get");
            Assert.AreEqual(urls.Domain, "https://api.vk.com/method/");
            Assert.AreEqual(urls.Groups, "groups.get");
            Assert.AreEqual(urls.Repost, "wall.repost");
            Assert.AreEqual(urls.UserInfo, "users.get");
            Assert.AreEqual(urls.GroupJoin, "groups.join");
            Assert.AreEqual(urls.LeaveGroup, "groups.leave");
            Assert.AreEqual(urls.MainDomain, "https://vk.com/");
            Assert.AreEqual(urls.WallSearch, "wall.search");
            Assert.AreEqual(urls.WallGetById, "wall.getById");
        }
    }
}