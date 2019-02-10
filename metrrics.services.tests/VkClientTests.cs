using System.Collections.Generic;
using metrics.Services.Abstract;
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
        private IVkClient _vkClient = TestOptions.GetClient();

        [TestMethod]
        public void TestLike()
        {
            var response = _vkClient.Like(new VkRepostViewModel() { Id = 16248, Owner_Id = -63771035 });
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

        [TestMethod]
        public void GetMessageTest()
        {
            var response = _vkClient.GetById(new List<VkRepostViewModel> {
                new VkRepostViewModel()
                {
                    Id = 16248,
                    Owner_Id = -63771035
                }
            });

            Assert.IsNotNull(response.Response);
            Assert.IsTrue(response.Response.Items.Count > 0);
            Assert.AreEqual(response.Response.Items[0].Owner_Id, -63771035);
            Assert.AreEqual(response.Response.Items[0].Id, 16248);
        }

        [TestMethod]
        public void GetGroupsTest()
        {
            var response = _vkClient.GetGroups(5, 0);
            Assert.IsNotNull(response.Response);
            Assert.IsTrue(response.Response.Items.Count == 5);
        }

        [TestMethod]
        public void GetUserInfoTest()
        {
            var userId = 68868143;
            var response = _vkClient.GetUserInfo(userId.ToString());
            Assert.IsNotNull(response.Response);
            Assert.IsTrue(response.Response.Count == 1);
            Assert.AreEqual(response.Response[0].Id, userId);
        }

        [TestMethod]
        public void GetUserInfoInvalidUserId()
        {
            var userId = "piama2006";
            var response = _vkClient.GetUserInfo(userId);
            Assert.IsNull(response.Response);
        }
    }
}