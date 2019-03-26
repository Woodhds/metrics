using System;
using System.Collections.Specialized;
using metrics.Services.Helpers;
using NUnit.Framework;

namespace metrics.services.tests
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void BuildUrlTestCheckNUll()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var nvc = new NameValueCollection();
                nvc.BuildUrl(null);
            });
        }

        [Test]
        public void BuildUrlValidUrlOneParam()
        {
            var nvc = new NameValueCollection()
            {
                { "s", "123" }
            };
            var url = nvc.BuildUrl("test");
            Assert.AreEqual(url.ToLower(), "http://test:80/?s=123");
        }

        [Test]
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

        [Test]
        public void BuildUrlInvalid()
        {
            Assert.Throws<UriFormatException>(() =>
            {
                NameValueCollection nvc = null;
                nvc.BuildUrl("ftp://test.\"ru");
            });
        }

        [Test]
        public void BuildUrlCollectionNull()
        {
            NameValueCollection nvc = null;
            var url = nvc.BuildUrl("test");
            Assert.AreEqual("http://test:80/", url.ToLower());
        }
    }
}
