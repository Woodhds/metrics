using System;
using metrics.Broker.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker.Kafka
{
    public class KafkaBrokerConfigurationBuilder : IBrokerConfigurationBuilder
    {
        public KafkaBrokerConfigurationBuilder(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IKafkaConfigurationProvider, KafkaConfigurationProvider>();
            serviceCollection.AddSingleton<IMessageBroker, KafkaMessageBroker>();
            serviceCollection.AddSingleton<IHandlerConfigurator, KafkaHandlerConfigurator>();
        }

        public BrokerConfiguration Build(IServiceProvider serviceProvider)
        {
            return new BrokerConfiguration(
                serviceProvider.GetRequiredService<IMessageBroker>(),
                serviceProvider.GetRequiredService<IHandlerConfigurator>(),
                ""
            );
        }
    }
}