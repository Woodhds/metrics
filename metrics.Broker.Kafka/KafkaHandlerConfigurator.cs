using System;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker.Kafka
{
    public class KafkaHandlerConfigurator : IHandlerConfigurator
    {
        private readonly IKafkaConfigurationProvider _kafkaConfigurationProvider;
        private readonly IServiceCollection _serviceProvider;

        public KafkaHandlerConfigurator(
            IKafkaConfigurationProvider kafkaConfigurationProvider,
            IServiceCollection serviceProvider
        )
        {
            _kafkaConfigurationProvider = kafkaConfigurationProvider;
            _serviceProvider = serviceProvider;
        }

        public void ConfigureConsumer<TEvent>() where TEvent : class
        {
            var consumer = _kafkaConfigurationProvider.GetConsumerConfig<TEvent>();
            
            /*
            consumer.Subscribe(nameof(TEvent));
            var handler = new KafkaMessageHandler<TEvent>(
                (IMessageHandler<TEvent>)
                _serviceProvider.GetService(typeof(IMessageHandler<TEvent>)),
                consumer
            );

            Task.Run(() => handler.Start());*/
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