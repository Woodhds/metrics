using Confluent.Kafka;
using Microsoft.Extensions.Options;

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
                        GroupId = typeof(T).Name + "-group",
                        AutoOffsetReset = AutoOffsetReset.Earliest
                    })
                .SetValueDeserializer(new KafkaSerializer<T>())
                .Build();
        }

        public IProducer<Null, T> GetProducerConfig<T>()
        {
            return new ProducerBuilder<Null, T>(new ProducerConfig
                {
                    BootstrapServers = _options.Value.Servers
                })
                .SetValueSerializer(new KafkaSerializer<T>())
                .Build();
        }
    }
}