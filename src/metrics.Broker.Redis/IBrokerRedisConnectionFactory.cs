using StackExchange.Redis;

namespace metrics.Broker.Redis
{
    public interface IBrokerRedisConnectionFactory
    {
        ConnectionMultiplexer Create();
    }
}