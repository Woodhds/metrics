using System;
using System.Threading;
using System.Threading.Tasks;
using metrics.EventSourcing.Abstractions.Query;
using metrics.EventSourcing.Exceptions;

namespace metrics.EventSourcing
{
    public class QueryProcessor : IQueryProcessor
    {
        private readonly IServiceProvider _resolver;

        public QueryProcessor(IServiceProvider resolver)
        {
            _resolver = resolver;
        }

        public Task<TResponse> ProcessAsync<TResponse>(IQuery<TResponse> query, CancellationToken token = default)
        {
            var queryHandlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
            var handler = _resolver.GetService(queryHandlerType);
            if (handler == null)
            {
                throw new QueryHandlerNotFoundException(queryHandlerType);
            }

            var method = queryHandlerType.GetMethod("ExecuteAsync");
            return (Task<TResponse>) method?.Invoke(handler, new object[] {query, token});
        }
    }
}