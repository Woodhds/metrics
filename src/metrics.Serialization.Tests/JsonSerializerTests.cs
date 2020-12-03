using System;
using System.Collections.Generic;
using System.IO;
using Base.Contracts;
using metrics.Serialization.Abstractions;
using metrics.Serialization.Tests.Models;
using NUnit.Framework;

namespace metrics.Serialization.Tests
{
    [TestFixture]
    public class JsonSerializerTests
    {
        private readonly IJsonSerializer _jsonSerializer;
        
        public JsonSerializerTests()
        {
            _jsonSerializer = new JsonSerializer(new JsonSerializerOptionsProvider());
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

        [Test]
        public void DeserializeMassiveObjectVk()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "testobjectvk.txt");
            FileAssert.Exists(path);
            var strings = File.ReadAllText(path);
            var result = _jsonSerializer.Deserialize<VkResponse<IEnumerable<VkMessage>>>(strings);
            Assert.NotNull(result?.Response);
            Assert.IsNotEmpty(result.Response.Items);
        }
    }
}