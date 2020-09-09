using System.Threading;
using System.Threading.Tasks;
using metrics.EventSourcing.Abstractions.Query;

namespace metrics.EventSourcing
{
    internal interface IQueryHandlerImpl
    {
        Task<TResponse> ExecuteAsync<TResponse>(IQueryHandler handler, IQuery query, CancellationToken token = default);
    }
    internal class QueryHandlerImpl<TQuery> : IQueryHandlerImpl where TQuery : IQuery
    {
        public async Task<TResponse> ExecuteAsync<TResponse>(IQueryHandler handler, IQuery query,
            CancellationToken token = default)
        {
            return await ((IQueryHandler<TQuery, TResponse>) handler).ExecuteAsync((TQuery) query, token)
                .ConfigureAwait(false);
        }
    }
}