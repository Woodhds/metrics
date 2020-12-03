using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;

namespace metrics.Broker.Nats
{
    public class NatsMessageBroker : IMessageBroker
    {
        private readonly INatsConnectionFactory _natsConnectionFactory;
        private readonly INatsSubjectProvider _natsSubjectProvider;
        private readonly INatsMessageSerializer _natsMessageSerializer;

        public NatsMessageBroker(INatsConnectionFactory natsConnectionFactory, INatsSubjectProvider natsSubjectProvider, INatsMessageSerializer natsMessageSerializer)
        {
            _natsConnectionFactory = natsConnectionFactory;
            _natsSubjectProvider = natsSubjectProvider;
            _natsMessageSerializer = natsMessageSerializer;
        }

        public Task PublishAsync<T>(T obj, CancellationToken token = default) where T : class, new()
        {
            using var connection = _natsConnectionFactory.CreateConnection();
            connection.Publish(_natsSubjectProvider.GetSubject<T>(), _natsMessageSerializer.Serialize(obj));

            return Task.CompletedTask;
        }

        public Task SendAsync<T>(T obj, CancellationToken token = default) where T : class, new()
        {
            return PublishAsync(obj, token);
        }
    }
}