using Microsoft.Extensions.Options;
using NATS.Client;

namespace metrics.Broker.Nats
{
    public interface INatsConnectionFactory
    {
        IConnection CreateConnection();
    }
    
    public class NatsConnectionFactory : INatsConnectionFactory
    {
        private readonly IOptions<NatsOptions> _options;

        public NatsConnectionFactory(IOptions<NatsOptions> options)
        {
            _options = options;
        }

        public IConnection CreateConnection()
        {
            var options = ConnectionFactory.GetDefaultOptions();
            options.Servers = _options.Value.Servers;
            options.ReconnectWait = 2000;

            var connection = new ConnectionFactory().CreateConnection(options);

            return connection;
        }
    }
}