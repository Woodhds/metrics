using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using metrics.Broker.Abstractions;

namespace metrics.Broker.Rabbitmq
{
    internal class MessageBroker : IMessageBroker
    {
        private readonly IBusControl _bus;

        public MessageBroker(IBusControl bus)
        {
            _bus = bus;
        }

        public Task PublishAsync<T>(T obj, CancellationToken token = default) where T : class, new()
        {
            return _bus.Publish(obj, token);
        }

        public Task SendAsync<T>(T obj, CancellationToken token = default) where T : class, new()
        {
            return _bus.Send(obj, token);
        }
    }
}