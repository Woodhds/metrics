using Confluent.Kafka;

namespace metrics.Broker.Kafka
{
    public interface IKafkaConfigurationProvider
    {
        IConsumer<Null, T> GetConsumerConfig<T>();
        IProducer<Null, T> GetProducerConfig<T>();
    }
}