using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using metrics.EventSourcing.Abstractions.Query;

namespace metrics.EventSourcing
{
    public class QueryProcessor : IQueryProcessor
    {
        private class CacheItem
        {
            public IQueryHandlerImpl HandlerImpl { get; set; }
            public IQueryHandler Handler { get; set; }
        }
        
        private readonly IServiceProvider _resolver;
        private readonly ConcurrentDictionary<Type, CacheItem> _implementations = new ConcurrentDictionary<Type, CacheItem>();

        public QueryProcessor(IServiceProvider resolver)
        {
            _resolver = resolver;
        }

        public async Task<TResponse> ProcessAsync<TResponse>(IQuery<TResponse> query, CancellationToken token = default)
        {
            var handler = GetHandler<TResponse>(query.GetType()); 

            return await handler.HandlerImpl.ExecuteAsync<TResponse>(handler.Handler, query, token);
        }

        private CacheItem GetHandler<TResponse>(Type queryType)
        {
            return _implementations.GetOrAdd(queryType, x =>
            {
                var queryHandlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse));
                return new CacheItem
                {
                    HandlerImpl =
                        (IQueryHandlerImpl) Activator.CreateInstance(typeof(QueryHandlerImpl<>).MakeGenericType(x)),
                    Handler = (IQueryHandler) _resolver.GetService(queryHandlerType)
                };
            });
        }
    }
}