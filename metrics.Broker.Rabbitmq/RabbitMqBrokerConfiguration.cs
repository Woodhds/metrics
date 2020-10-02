using System;
using MassTransit;
using metrics.Broker.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker.Rabbitmq
{
    public class RabbitMqBrokerConfigurationBuilder : IBrokerConfigurationBuilder
    {
        private readonly AmqpOptions _options;
        private readonly IBusControl _busControl;
        private readonly IMessageBroker _messageBroker;
        private readonly IServiceCollection _serviceCollection;

        public RabbitMqBrokerConfigurationBuilder(IConfiguration configuration, IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            _options = new AmqpOptions();
            configuration.GetSection(nameof(AmqpOptions)).Bind(_options);
            _busControl = _options.InMemory ? CreateUsingInMemory() : CreateUsingRabbitmq(_options);
            _messageBroker = new MessageBroker(_busControl);
            serviceCollection.AddSingleton(typeof(IMessageBroker), _ => _messageBroker);
            
            serviceCollection.AddHostedService<MessageBrokerHostedService>();
            serviceCollection.AddSingleton(_busControl);
        }

        public BrokerConfiguration Build()
        {
            return new BrokerConfiguration(_messageBroker, new HandlerConfigurator(_serviceCollection, _busControl),
                _options.Host);
        }

        private static IBusControl CreateUsingRabbitmq(AmqpOptions options)
        {
            return Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.Host(new Uri($"{options.Host}"), configurator =>
                {
                    configurator.Username(options.UserName);
                    configurator.Password(options.Password);
                });
            });
        }

        private static IBusControl CreateUsingInMemory()
        {
            Console.WriteLine("Bus started with InMemory transport");
            return Bus.Factory.CreateUsingInMemory(x => { x.TransportConcurrencyLimit = 100; });
        }
    }
}