using System;
using System.Threading;
using System.Threading.Tasks;
using metrics.Broker.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace metrics.Broker.Kafka
{
    public class KafkaHostedHandler<TEvent> : BackgroundService where TEvent : class
    {
        private readonly IKafkaConfigurationProvider _kafkaConfigurationProvider;
        private readonly IMessageHandler<TEvent> _messageHandler;
        private readonly ILogger<KafkaHostedHandler<TEvent>> _logger;
        private readonly IServiceProvider _serviceProvider;

        public KafkaHostedHandler(
            IKafkaConfigurationProvider kafkaConfigurationProvider,
            IMessageHandler<TEvent> messageHandler,
            ILogger<KafkaHostedHandler<TEvent>> logger,
            IServiceProvider serviceProvider
        )
        {
            _kafkaConfigurationProvider = kafkaConfigurationProvider;
            _messageHandler = messageHandler;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            using var consumer = _kafkaConfigurationProvider.GetConsumerConfig<TEvent>();
            consumer.Subscribe(nameof(TEvent));

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        using var consumer = _kafkaConfigurationProvider.GetConsumerConfig<TEvent>();
                        var consumption = consumer.Consume(stoppingToken);
                        if (consumption.Message.Value != null)
                        {
                            using var scope = _serviceProvider.CreateScope();
                            
                            await _messageHandler.HandleAsync(consumption.Message.Value, stoppingToken);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(e.Message);
                    }
                }
            }, stoppingToken).ConfigureAwait(false);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _kafkaConfigurationProvider.GetConsumerConfig<TEvent>().Dispose();
            return base.StopAsync(cancellationToken);
        }
    }
}