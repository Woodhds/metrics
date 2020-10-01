using System;
using metrics.Broker.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker.Kafka
{
    public class KafkaBrokerConfigurationBuilder : IBrokerConfigurationBuilder
    {
        public KafkaBrokerConfigurationBuilder(IConfiguration configuration, IServiceCollection serviceCollection)
        {
            serviceCollection.Configure<KafkaConfiguration>(configuration.GetSection(nameof(KafkaConfiguration)));
            serviceCollection.AddSingleton<IKafkaConfigurationProvider, KafkaConfigurationProvider>();
            serviceCollection.AddSingleton<IMessageBroker, KafkaMessageBroker>();
            serviceCollection.AddSingleton<IHandlerConfigurator, KafkaHandlerConfigurator>();
        }

        public BrokerConfiguration Build(IServiceCollection serviceProvider)
        {
            var provider = serviceProvider.BuildServiceProvider();
            return new BrokerConfiguration(
                provider.GetRequiredService<IMessageBroker>(),
                provider.GetRequiredService<IHandlerConfigurator>(),
                ""
            );
        }
    }
}