using System.Threading;
using System.Threading.Tasks;

namespace metrics.EventSourcing.Abstractions.Query
{
    public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
    {
        Task<TResponse> ExecuteAsync(TQuery query, CancellationToken token = default);
    }
}