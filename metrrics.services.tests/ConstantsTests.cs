using NUnit.Framework;

namespace metrics.services.tests
{
    [TestFixture]
    public class ConstantsTests
    {
        [Test]
        public void ConstantTest()
        {
            Assert.AreEqual(Constants.ApiVersion, "5.85");
            Assert.AreEqual(Constants.VK_TOKEN_CLAIM, "VkToken");
        }
    }
}