using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace metrics.services.test
{
    [TestClass]
    public class ConstantsTests
    {
        public void VkTokenTest()
        {
            Assert.AreEqual("VkToken", Constants.VK_TOKEN_CLAIM);
        }

        public void VkVersionTest()
        {
            Assert.AreEqual("5.85", Constants.ApiVersion);
        }
    }
}
