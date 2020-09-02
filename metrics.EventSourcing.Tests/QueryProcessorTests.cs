using System.Threading;
using metrics.EventSourcing.Abstractions.Query;
using metrics.EventSourcing.Tests.Events;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace metrics.EventSourcing.Tests
{
    [TestFixture]
    public class QueryProcessorTests
    {
        private readonly IQueryProcessor _queryProcessor; 
        
        public QueryProcessorTests()
        {
            _queryProcessor = Initializer.ServiceProvider.GetRequiredService<IQueryProcessor>();
        }
        
        [Test]
        public void ProcessAsyncTest()
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                var result = await _queryProcessor.ProcessAsync(new TestQuery(), CancellationToken.None);
                Assert.NotNull(result);
            });
        }
    }
}