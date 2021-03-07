using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using StackExchange.Redis;

namespace metrics.Broker.Redis
{
    public class RedisMessageHandler<T> where T : class, new()
    {
        private readonly IMessageHandler<T> _handler;
        private readonly IRedisMessageSerializer  _messageSerializer;

        public RedisMessageHandler(IMessageHandler<T> handler, IRedisMessageSerializer messageSerializer)
        {
            _handler = handler;
            _messageSerializer = messageSerializer;
        }
        
        public Task HandleAsync(RedisValue msg)
        {
            return _handler.HandleAsync(_messageSerializer.Deserialize<T>(msg));
        }
    }
}