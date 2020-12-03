using System.Threading;
using System.Threading.Tasks;

namespace metrics.EventSourcing.Abstractions.Query
{
    public interface IQueryHandler
    {
        
    }
    
    public interface IQueryHandler<in TQuery, TResponse> : IQueryHandler where TQuery : IQuery
    {
        Task<TResponse> ExecuteAsync(TQuery query, CancellationToken token = default);
    }
}