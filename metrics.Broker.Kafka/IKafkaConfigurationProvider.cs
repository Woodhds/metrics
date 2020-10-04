using Confluent.Kafka;

namespace metrics.Broker.Kafka
{
    public interface IKafkaConfigurationProvider
    {
        IConsumer<Null, T> GetConsumerConfig<T>() where T: class, new();
        IProducer<Null, T> GetProducerConfig<T>();
    }
}