using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using metrics.Broker.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace metrics.Broker.Kafka
{
    public class KafkaHostedHandler<TEvent> : BackgroundService where TEvent : class, new()
    {
        private readonly IMessageHandler<TEvent> _messageHandler;
        private readonly ILogger<KafkaHostedHandler<TEvent>> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConsumer<Null, TEvent> _consumer;

        public KafkaHostedHandler(
            IKafkaConfigurationProvider kafkaConfigurationProvider,
            IMessageHandler<TEvent> messageHandler,
            ILogger<KafkaHostedHandler<TEvent>> logger,
            IServiceProvider serviceProvider
        )
        {
            _messageHandler = messageHandler;
            _logger = logger;
            _consumer = kafkaConfigurationProvider.GetConsumerConfig<TEvent>();
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(typeof(TEvent).Name);

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumption = _consumer.Consume(stoppingToken);
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
            }, stoppingToken);
            
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Unsubscribe();
            return base.StopAsync(cancellationToken);
        }
    }
}