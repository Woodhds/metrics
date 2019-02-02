using metrics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace metrrics.services.tests
{
    [TestClass]
    public class ConstantsTests
    {
        [TestMethod]
        public void ConstantTest()
        {
            Assert.AreEqual(Constants.ApiVersion, "5.85");
            Assert.AreEqual(Constants.VK_TOKEN_CLAIM, "VkToken");
        }
    }
}