using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using metrics.Broker.Abstractions;

namespace metrics.Broker.Kafka
{
    public class KafkaMessageBroker : IMessageBroker
    {
        private readonly IKafkaConfigurationProvider _kafkaConfigurationProvider;

        public KafkaMessageBroker(IKafkaConfigurationProvider kafkaConfigurationProvider)
        {
            _kafkaConfigurationProvider = kafkaConfigurationProvider;
        }

        public Task PublishAsync<T>(T obj, CancellationToken token = default) where T : class
        {
            return _kafkaConfigurationProvider.GetProducerConfig<T>()
                .ProduceAsync(typeof(T).Name, new Message<Null, T> {Value = obj}, token);
        }

        public Task SendAsync<T>(T obj, CancellationToken token = default) where T : class
        {
            return PublishAsync(obj, token);
        }
    }
}