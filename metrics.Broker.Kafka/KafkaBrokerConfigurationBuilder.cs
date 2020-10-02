using System;
using metrics.Broker.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker.Kafka
{
    public class KafkaBrokerConfigurationBuilder : IBrokerConfigurationBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public KafkaBrokerConfigurationBuilder(IConfiguration configuration, IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            serviceCollection.Configure<KafkaConfiguration>(configuration.GetSection(nameof(KafkaConfiguration)));
            serviceCollection.AddSingleton<IKafkaConfigurationProvider, KafkaConfigurationProvider>();
            serviceCollection.AddSingleton<IMessageBroker, KafkaMessageBroker>();
            
        }

        public BrokerConfiguration Build()
        {
            var provider = _serviceCollection.BuildServiceProvider();
            return new BrokerConfiguration(
                provider.GetRequiredService<IMessageBroker>(),
                new KafkaHandlerConfigurator(provider.GetRequiredService<IKafkaConfigurationProvider>(), _serviceCollection), 
                ""
            );
        }
    }
}