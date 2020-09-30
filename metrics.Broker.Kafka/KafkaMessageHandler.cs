using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using metrics.Broker.Abstractions;

namespace metrics.Broker.Kafka
{
    public class KafkaMessageHandler<T> where T : class
    {
        private readonly IMessageHandler<T> _handler;
        private readonly IConsumer<Null, T> _consumer;

        public KafkaMessageHandler(
            IMessageHandler<T> handler,
            IConsumer<Null, T> consumer
        )
        {
            _handler = handler;
            _consumer = consumer;
        }

        public async Task Start(CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                var consumption = _consumer.Consume(token);
                if (consumption.Message.Value != default)
                {
                    await _handler.HandleAsync(consumption.Message.Value, token);
                }
            }
        }
    }
}