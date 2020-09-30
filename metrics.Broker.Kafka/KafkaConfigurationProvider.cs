using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace metrics.Broker.Kafka
{
    public class KafkaConfigurationProvider : IKafkaConfigurationProvider
    {
        private readonly IOptions<KafkaConfiguration> _options;

        public KafkaConfigurationProvider(IOptions<KafkaConfiguration> options)
        {
            _options = options;
        }

        public IConsumer<Null, T> GetConsumerConfig<T>()
        {
            return new ConsumerBuilder<Null, T>(
                    new ConsumerConfig
                    {
                        BootstrapServers = _options.Value.Servers,
                        GroupId = nameof(T) + "-group"
                    })
                .Build();
        }

        public IProducer<Null, T> GetProducerConfig<T>()
        {
            return new ProducerBuilder<Null, T>(new ProducerConfig
                {
                    BootstrapServers = _options.Value.Servers
                })
                .Build();
        }
    }
}