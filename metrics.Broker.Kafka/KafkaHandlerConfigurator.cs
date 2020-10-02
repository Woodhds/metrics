using metrics.Broker.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker.Kafka
{
    public class KafkaHandlerConfigurator : IHandlerConfigurator
    {
        private readonly IKafkaConfigurationProvider _kafkaConfigurationProvider;
        private readonly IServiceCollection _serviceCollection;

        public KafkaHandlerConfigurator(
            IKafkaConfigurationProvider kafkaConfigurationProvider,
            IServiceCollection serviceCollection
        )
        {
            _kafkaConfigurationProvider = kafkaConfigurationProvider;
            _serviceCollection = serviceCollection;
        }

        public void ConfigureConsumer<TEvent>() where TEvent : class
        {
            var consumer = _kafkaConfigurationProvider.GetConsumerConfig<TEvent>();
            consumer.Subscribe(nameof(TEvent));

            _serviceCollection.AddHostedService<KafkaHostedHandler<TEvent>>();
        }

        public void ConfigureCommandConsumer<TEvent>() where TEvent : class
        {
            ConfigureConsumer<TEvent>();
        }

        public void ConfigureCommand<TCommand>(string host) where TCommand : class
        {
        }
    }
}