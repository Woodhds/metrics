using System.Threading.Tasks;

namespace metrics.EventSourcing.Abstractions.Query
{
    public interface IQueryProcessor
    {
        Task<TResponse> ProcessAsync<TResponse>(IQuery<TResponse> query);
        Task<TResponse> ProcessAsync<TResponse>(IQuery query);
    }
}