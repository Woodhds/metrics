using System.Threading;
using System.Threading.Tasks;

namespace metrics.Broker.Abstractions
{
    public interface IMessageHandler
    {
    }
    
    public interface IMessageHandler<in T> : IMessageHandler where T : class
    {
        Task HandleAsync(T obj, CancellationToken token = default);
    }
}