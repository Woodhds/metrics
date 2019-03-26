using System.Collections.Generic;
using metrics.Services.Abstract;
using metrics.Services.Models;
using NUnit.Framework;

namespace metrics.services.tests
{
    [TestFixture]
    public class VkClientTests
    {
        private IVkClient _vkClient = TestOptions.GetClient();

        [Test]
        public void TestLike()
        {
            var response = _vkClient.Like(new VkRepostViewModel() { Id = 16248, Owner_Id = -63771035 });
            Assert.IsNotNull(response?.Response);
        }

        [Test]
        public void TestRepost()
        {
            var response = _vkClient.Repost(new List<VkRepostViewModel>()
                {new VkRepostViewModel {Id = 16248, Owner_Id = -63771035}});
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Count == 1);
            Assert.IsNotNull(response[0].Response);
            Assert.IsTrue(response[0].Response.Success);
        }

        [Test]
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

        [Test]
        public void GetGroupsTest()
        {
            var response = _vkClient.GetGroups(5, 0);
            Assert.IsNotNull(response.Response);
            Assert.IsTrue(response.Response.Items.Count == 5);
        }

        [Test]
        public void GetUserInfoTest()
        {
            var userId = 68868143;
            var response = _vkClient.GetUserInfo(userId.ToString());
            Assert.IsNotNull(response.Response);
            Assert.IsTrue(response.Response.Count == 1);
            Assert.AreEqual(response.Response[0].Id, userId);
        }

        [Test]
        public void GetUserInfoInvalidUserId()
        {
            var userId = "piama2006";
            var response = _vkClient.GetUserInfo(userId);
            Assert.IsNull(response.Response);
        }
    }
}