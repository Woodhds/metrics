using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using NATS.Client;

namespace metrics.Broker.Nats
{
    public class NatsMessageHandler<T> where T : class, new()
    {
        private readonly IMessageHandler<T> _handler;
        private readonly INatsMessageSerializer _natsMessageSerializer;

        public NatsMessageHandler(IMessageHandler<T> handler, INatsMessageSerializer natsMessageSerializer)
        {
            _handler = handler;
            _natsMessageSerializer = natsMessageSerializer;
        }

        public Task HandleAsync(Msg msg)
        {
            return _handler.HandleAsync(_natsMessageSerializer.Deserialize<T>(msg.Data));
        }
    }
}