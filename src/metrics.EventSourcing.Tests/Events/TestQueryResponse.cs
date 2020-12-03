using metrics.EventSourcing.Abstractions.Query;

namespace metrics.EventSourcing.Tests.Events
{
    public class TestQueryResponse : IQuery<int>
    {
        public string SearchQuery { get; set; }
    }
}