using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
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

        public IConsumer<Null, T> GetConsumerConfig<T>() where T : class, new()
        {
            return new ConsumerBuilder<Null, T>(
                    new ConsumerConfig
                    {
                        BootstrapServers = _options.Value.Servers,
                        GroupId = typeof(T).Name + "-group",
                        AutoOffsetReset = AutoOffsetReset.Earliest
                    })
                .SetValueDeserializer(new KafkaProtobufSerializer<T>())
                .Build();
        }

        public IProducer<Null, T> GetProducerConfig<T>() where T: class, new()
        {
            return new ProducerBuilder<Null, T>(new ProducerConfig
                {
                    BootstrapServers = _options.Value.Servers
                })
                .SetValueSerializer(new KafkaProtobufSerializer<T>())
                .Build();
        }
    }
}