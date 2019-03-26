using NUnit.Framework;

namespace metrics.services.tests
{
    [TestFixture]
    public class VkApiUrlsTest
    {
        [Test]
        public void UrlsTest()
        {
            var urls = TestOptions.GetUrls();
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