using System.Threading;
using System.Threading.Tasks;

namespace metrics.Broker.Abstractions
{
    public interface IMessageBroker
    {
        Task PublishAsync<T>(T obj, CancellationToken token = default) where T : class;
    }
}