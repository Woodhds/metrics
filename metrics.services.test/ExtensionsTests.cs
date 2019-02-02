using metrics.Services.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;

namespace metrics.services.test
{
    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        public void BuildUrlTestCheckNUll()
        {
            var nvc = new NameValueCollection();
            var url = nvc.BuildUrl(null);
            Assert.AreEqual(url, string.Empty);
        }

        [TestMethod]
        public void BuildUrlValidUrlOneParam()
        {
            var nvc = new NameValueCollection()
            {
                { "s", "123" }
            };
            var url = nvc.BuildUrl("test");
            Assert.AreEqual(url.ToLower(), "http://test:80/?s=123");
        }

        [TestMethod]
        public void BuildUrlValidUrlSeveralParams()
        {
            var nvc = new NameValueCollection()
            {
                { "param1", "test" },
                { "param2", "test2" }
            };
            var url = nvc.BuildUrl("test");
            Assert.AreEqual(url.ToLower(), "http://test:80/?param1=test&param2=test2");
        }

        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void BuildUrlInvalid()
        {
            NameValueCollection nvc = null;
            nvc.BuildUrl("ftp://test.\"ru");
        }

        [TestMethod]
        public void BuildUrlCollectionNull()
        {
            NameValueCollection nvc = null;
            var url = nvc.BuildUrl("test");
            Assert.AreEqual("http://test:80/", url.ToLower());
        }
    }
}
