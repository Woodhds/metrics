using System;
using metrics.Serialization.Abstractions;
using metrics.Serialization.Tests.Models;
using NUnit.Framework;

namespace metrics.Serialization.Tests
{
    [TestFixture]
    public class JsonSerializerTests
    {
        private IJsonSerializer _jsonSerializer;
        
        public JsonSerializerTests()
        {
            _jsonSerializer = new JsonSerializer();
        }
        
        [Test]
        public void SerializeTest()
        {
            var str = _jsonSerializer.Serialize(new TestSerializationObject
            {
                Date = DateTime.Now
            });

            Assert.IsNotEmpty(str);
        }

        [Test]
        public void DateSerializationTest()
        {
            var obj = new TestSerializationObject
            {
                Date = new DateTime(2020, 6, 6, 12, 15, 0, DateTimeKind.Utc)
            };
            var result = _jsonSerializer.Serialize(obj);
            
            Assert.IsTrue(result.Contains("1591445700"));
            Assert.AreEqual(_jsonSerializer.Deserialize<TestSerializationObject>(result).Date, obj.Date);
        }
    }
}