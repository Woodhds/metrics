using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;

namespace metrics.Broker.Redis
{
    public class RedisMessageBroker : IMessageBroker
    {
        private readonly IBrokerRedisConnectionFactory _brokerRedisConnectionFactory;
        private readonly IRedisMessageSerializer _messageSerializer;

        public RedisMessageBroker(
            IBrokerRedisConnectionFactory brokerRedisConnectionFactory,
            IRedisMessageSerializer messageSerializer)
        {
            _brokerRedisConnectionFactory = brokerRedisConnectionFactory;
            _messageSerializer = messageSerializer;
        }

        public Task PublishAsync<T>(T obj, CancellationToken token = default) where T : class, new()
        {
            return _brokerRedisConnectionFactory.Create().GetSubscriber()
                .PublishAsync(typeof(T).FullName, _messageSerializer.Serialize(obj));
        }

        public Task SendAsync<T>(T obj, CancellationToken token = default) where T : class, new()
        {
            return PublishAsync(obj, token);
        }
    }
}