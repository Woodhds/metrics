using System.Collections.Generic;
using metrics.Services.Concrete;
using metrics.Services.Models;
using metrics.Services.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace metrrics.services.tests
{
    [TestClass]
    public class VkClientTests
    {
        private VkClient _vkClient;
        [TestInitialize]
        public void Setup()
        {
            var urls = TestOptions.GetUrls();
            _vkClient = new VkClient(new TestHttpClientFactory(), new HttpContextAccessor {HttpContext = new TestHttpContext()},
                new OptionsWrapper<VKApiUrls>(urls), new NullLogger<VkClient>());
        }
        
        [TestMethod]
        public void TestLike()
        {
            var response = _vkClient.Like(new VkRepostViewModel() {Id = 16248, Owner_Id = -63771035});
            Assert.IsNotNull(response?.Response);
        }

        [TestMethod]
        public void TestRepost()
        {
            var response = _vkClient.Repost(new List<VkRepostViewModel>()
                {new VkRepostViewModel {Id = 16248, Owner_Id = -63771035}});
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Count == 1);
            Assert.IsNotNull(response[0].Response);
            Assert.IsTrue(response[0].Response.Success);
        }
    }
}