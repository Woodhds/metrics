using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;

namespace metrics.Broker.Kafka
{
    public class KafkaMessageHandler<T> where T : class
    {
        private readonly IMessageHandler<T> _handler;
        private readonly IKafkaConfigurationProvider _kafkaConfigurationProvider;

        public KafkaMessageHandler(
            IMessageHandler<T> handler,
            IKafkaConfigurationProvider kafkaConfigurationProvider
        )
        {
            _handler = handler;
            _kafkaConfigurationProvider = kafkaConfigurationProvider;
        }

        public async Task Start(CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                using var consumer = _kafkaConfigurationProvider.GetConsumerConfig<T>();
                var consumption = consumer.Consume(token);
                if (consumption.Message.Value != default)
                {
                    await _handler.HandleAsync(consumption.Message.Value, token);
                }
            }
        }
    }
}