using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using Microsoft.Extensions.Hosting;

namespace metrics.Broker.Redis
{
    public class RedisHostedHandler<TEvent> : IHostedService where TEvent : class, new()
    {
        private readonly IBrokerRedisConnectionFactory _brokerRedisConnectionFactory;
        private readonly IRedisMessageSerializer _messageSerializer;
        private readonly IMessageHandler<TEvent> _messageHandler;

        public RedisHostedHandler(IBrokerRedisConnectionFactory brokerRedisConnectionFactory, IRedisMessageSerializer messageSerializer, IMessageHandler<TEvent> messageHandler)
        {
            _brokerRedisConnectionFactory = brokerRedisConnectionFactory;
            _messageSerializer = messageSerializer;
            _messageHandler = messageHandler;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _brokerRedisConnectionFactory.Create().GetSubscriber()
                .SubscribeAsync(typeof(TEvent).FullName,
                    (_, value) =>
                        new RedisMessageHandler<TEvent>(_messageHandler, _messageSerializer).HandleAsync(value));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _brokerRedisConnectionFactory.Create().GetSubscriber().UnsubscribeAsync(typeof(TEvent).FullName);
        }
    }
}