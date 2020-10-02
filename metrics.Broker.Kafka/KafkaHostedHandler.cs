using System;
using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace metrics.Broker.Kafka
{
    public class KafkaHostedHandler<TEvent> : BackgroundService where TEvent : class
    {
        private readonly IKafkaConfigurationProvider _kafkaConfigurationProvider;
        private readonly IMessageHandler<TEvent> _messageHandler;
        private readonly ILogger<KafkaHostedHandler<TEvent>> _logger;

        public KafkaHostedHandler(IKafkaConfigurationProvider kafkaConfigurationProvider,
            IMessageHandler<TEvent> messageHandler, ILogger<KafkaHostedHandler<TEvent>> logger)
        {
            _kafkaConfigurationProvider = kafkaConfigurationProvider;
            _messageHandler = messageHandler;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var consumer = _kafkaConfigurationProvider.GetConsumerConfig<TEvent>();
                    var consumption = consumer.Consume(stoppingToken);
                    if (consumption.Message.Value != null)
                    {
                        await _messageHandler.HandleAsync(consumption.Message.Value, stoppingToken);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e.Message);
                }
            }
        }
    }
}