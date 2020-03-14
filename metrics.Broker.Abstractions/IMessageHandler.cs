using System.Threading;
using System.Threading.Tasks;

namespace metrics.Broker.Abstractions
{
    public interface IMessageHandler<in T> where T : class
    {
        Task HandleAsync(T obj, CancellationToken token = default);
    }
}