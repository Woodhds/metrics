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
        [TestMethod]
        public void TestMethod1()
        {
            var client = new VkClient(new TestHttpClientFactory(), new HttpContextAccessor(),
                new OptionsWrapper<VKApiUrls>(new VKApiUrls()), new NullLogger<VkClient>());
            client.Like(new VkRepostViewModel() {Id = 34, Owner_Id = -542323});
        }
    }
}