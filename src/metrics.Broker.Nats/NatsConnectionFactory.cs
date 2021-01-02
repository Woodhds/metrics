using metrics.Broker.Nats.Pooling;

namespace metrics.Broker.Nats
{
    public interface INatsConnectionFactory
    {
        INatsConnection CreateConnection();
    }
    
    public class NatsConnectionFactory : INatsConnectionFactory
    {
        private readonly INatsPool _natsPool;

        public NatsConnectionFactory(INatsPool natsPool)
        {
            _natsPool = natsPool;
        }

        public INatsConnection CreateConnection() => _natsPool.Rent();
    }
}