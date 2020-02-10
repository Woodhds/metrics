using System.Threading.Tasks;
using Base.Abstractions;
using Base.Contracts;
using Base.Contracts.Options;
using Microsoft.Extensions.Options;
using Moq;
using Nest;
using NUnit.Framework;

namespace Elastic.Tests
{
    public class ElasticTests
    {
        private IElasticClient _elasticClientProvider; 
        [SetUp]
        public void Setup()
        {
            var mock = new Mock<IOptions<ElasticOptions>>();
            mock.SetupGet(options => options.Value)
                .Returns(new ElasticOptions() {Host = "http://localhost:9200"});
            _elasticClientProvider = new ElasticClientFactory(mock.Object).Create();
        }

        [Test]
        public async Task Test1()
        {
            var result = await _elasticClientProvider.SearchAsync<VkMessage>();
            Assert.IsTrue(result.IsValid);
            Assert.IsNotEmpty(result.Documents);
        }
    }
}