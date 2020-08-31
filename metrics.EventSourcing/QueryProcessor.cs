using System.Threading.Tasks;
using metrics.core.Services;
using metrics.EventSourcing.Abstractions.Query;

namespace metrics.EventSourcing
{
    public class QueryProcessor : IQueryProcessor
    {
        private readonly IServiceResolver _resolver;

        public QueryProcessor(IServiceResolver resolver)
        {
            _resolver = resolver;
        }

        public Task<TResponse> ProcessAsync<TResponse>(IQuery<TResponse> query)
        {
            throw new System.NotImplementedException();
        }

        public Task<TResponse> ProcessAsync<TResponse>(IQuery query)
        {
            throw new System.NotImplementedException();
        }
    }
}