using System.Threading;
using System.Threading.Tasks;

namespace metrics.EventSourcing.Abstractions.Query
{
    public interface IQueryProcessor
    {
        Task<TResponse> ProcessAsync<TResponse>(IQuery<TResponse> query, CancellationToken token = default);
    }
}