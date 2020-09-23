using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using metrics.EventSourcing.Abstractions.Query;
using metrics.EventSourcing.Exceptions;

namespace metrics.EventSourcing
{
    public class QueryProcessor : IQueryProcessor
    {
        private class CacheItem
        {
            public readonly IQueryHandlerImpl HandlerImpl;
            public readonly IQueryHandler Handler;

            public CacheItem(IQueryHandlerImpl handlerImpl, IQueryHandler handler)
            {
                HandlerImpl = handlerImpl;
                Handler = handler;
            }
        }

        private readonly IServiceProvider _resolver;

        private readonly ConcurrentDictionary<Type, CacheItem> _implementations =
            new ConcurrentDictionary<Type, CacheItem>();

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
                return new CacheItem(
                    (IQueryHandlerImpl) Activator.CreateInstance(typeof(QueryHandlerImpl<>).MakeGenericType(x)),
                    (IQueryHandler) (_resolver.GetService(queryHandlerType) ??
                                     throw new QueryHandlerNotFoundException(queryHandlerType)));
            });
        }
    }
}