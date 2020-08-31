using System.Threading;
using System.Threading.Tasks;

namespace metrics.EventSourcing.Abstractions.Query
{
    public interface IQueryHandler<in TQuery> where TQuery : IQuery
    {
        Task<TResponse> ExecuteAsync<TResponse>(IQuery<TResponse> query, CancellationToken token);
    }
}