using System;
using System.Threading;
using System.Threading.Tasks;
using metrics.EventSourcing.Abstractions.Query;

namespace metrics.EventSourcing.Tests
{
    public class TestQueryProcessor : IQueryProcessor
    {
        private readonly IServiceProvider _resolver;

        public TestQueryProcessor(IServiceProvider resolver)
        {
            _resolver = resolver;
        }

        public Task<TResponse> ProcessAsync<TResponse>(IQuery<TResponse> query, CancellationToken token = default)
        {
            var queryHandlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
            var handler = _resolver.GetService(queryHandlerType);
            if (handler == null)
            {
                throw new ArgumentException();
            }

            
            var method = queryHandlerType.GetMethod("ExecuteAsync");
            return (Task<TResponse>) method?.Invoke(handler, new object[] {query, token});
        }
    }
}