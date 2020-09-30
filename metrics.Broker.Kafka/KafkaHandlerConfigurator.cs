using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using metrics.Broker.Abstractions;

namespace metrics.Broker.Kafka
{
    public class KafkaHandlerConfigurator : IHandlerConfigurator
    {
        private readonly IKafkaConfigurationProvider _kafkaConfigurationProvider;
        private readonly IServiceProvider _serviceProvider;

        public KafkaHandlerConfigurator(
            IKafkaConfigurationProvider kafkaConfigurationProvider,
            IServiceProvider serviceProvider
        )
        {
            _kafkaConfigurationProvider = kafkaConfigurationProvider;
            _serviceProvider = serviceProvider;
        }

        public void ConfigureConsumer<TEvent>() where TEvent : class
        {
            var consumer = _kafkaConfigurationProvider.GetConsumerConfig<TEvent>();
            
            consumer.Subscribe(nameof(TEvent));
            var handler = new KafkaMessageHandler<TEvent>(
                (IMessageHandler<TEvent>)
                _serviceProvider.GetService(typeof(IMessageHandler<TEvent>)),
                consumer
            );

            Task.Run(() => handler.Start());
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