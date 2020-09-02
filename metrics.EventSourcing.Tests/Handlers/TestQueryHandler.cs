using System.Threading;
using System.Threading.Tasks;
using metrics.EventSourcing.Abstractions.Query;
using metrics.EventSourcing.Tests.Events;

namespace metrics.EventSourcing.Tests.Handlers
{
    public class TestQueryHandler : IQueryHandler<TestQuery, string>
    {
        public Task<string> ExecuteAsync(TestQuery query, CancellationToken token)
        {
            return Task.FromResult($"{query.Date:d}: {query.Price:F}");
        }
    }
}