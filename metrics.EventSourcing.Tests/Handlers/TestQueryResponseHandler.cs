using System.Threading;
using System.Threading.Tasks;
using metrics.EventSourcing.Abstractions.Query;
using metrics.EventSourcing.Tests.Events;

namespace metrics.EventSourcing.Tests.Handlers
{
    public class TestQueryResponseHandler : IQueryHandler<TestQueryResponse, int>
    {
        public Task<int> ExecuteAsync(TestQueryResponse query, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }
    }
}