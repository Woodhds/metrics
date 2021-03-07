using StackExchange.Redis;

namespace metrics.Broker.Redis
{
    public class BrokerRedisConnectionFactory : IBrokerRedisConnectionFactory
    {
        private ConnectionMultiplexer _connectionMultiplexer;

        public BrokerRedisConnectionFactory(ConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public ConnectionMultiplexer Create()
        {
            return _connectionMultiplexer;
        }
    }
}