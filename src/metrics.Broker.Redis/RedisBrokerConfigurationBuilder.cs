using metrics.Broker.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace metrics.Broker.Redis
{
    public class RedisBrokerConfigurationBuilder : IBrokerConfigurationBuilder
    {
        private readonly IServiceCollection _serviceCollection;
        
        public RedisBrokerConfigurationBuilder(IConfiguration configuration, IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            var options = new RedisBrokerOptions();
            configuration.GetSection(nameof(RedisBrokerOptions)).Bind(options);
            var connection = ConnectionMultiplexer.Connect(options.Configuration);
            serviceCollection.AddSingleton<IBrokerRedisConnectionFactory>(new BrokerRedisConnectionFactory(connection));
            serviceCollection.AddSingleton<IProtobufMessageSerializer, ProtobufMessageSerializer>();
            serviceCollection.AddSingleton<IRedisMessageSerializer, RedisMessageSerializer>();
            serviceCollection.AddSingleton<IMessageBroker, RedisMessageBroker>();
        }
        public BrokerConfiguration Build()
        {
            var provider = _serviceCollection.BuildServiceProvider();
            return new BrokerConfiguration(provider.GetService<IMessageBroker>(),
                new RedisHandlerConfigurator(_serviceCollection), "");
        }
    }
}