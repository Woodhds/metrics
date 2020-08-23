using System;
using MassTransit;
using metrics.Broker.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMessageBroker(
            this IServiceCollection serviceCollection,
            IConfiguration configuration,
            Action<IMessageHandlerProvider> handlerProvider = null
        )
        {
            var options = new AmqpOptions();
            configuration.GetSection(nameof(AmqpOptions)).Bind(options);

            var bus = options.InMemory ? CreateUsingInMemory() : CreateUsingRabbitmq(options);

            serviceCollection.AddSingleton(bus);
            serviceCollection.AddSingleton<IMessageBroker, MessageBroker>(_ => new MessageBroker(bus));

            var hp = new MessageHandlerProvider(serviceCollection);

            handlerProvider?.Invoke(hp);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            
            var handlerConfigurator = new HandlerConfigurator(serviceProvider, bus);
            
            foreach (var (tEvent, _) in hp.GetConsumers())
            {
                var method = typeof(HandlerConfigurator).GetMethod(nameof(HandlerConfigurator.ConfigureConsumer))
                    ?.MakeGenericMethod(tEvent);
                method?.Invoke(handlerConfigurator, null);
            }

            foreach (var command in hp.GetCommands())
            {
                var method = typeof(HandlerConfigurator).GetMethod(nameof(HandlerConfigurator.ConfigureCommand))
                    ?.MakeGenericMethod(command);
                method?.Invoke(handlerConfigurator, new object?[]{options.Host});
            }
            
            foreach (var (tEvent, _) in hp.GetCommandConsumers())
            {
                var method = typeof(HandlerConfigurator).GetMethod(nameof(HandlerConfigurator.ConfigureCommandConsumer))
                    ?.MakeGenericMethod(tEvent);
                method?.Invoke(handlerConfigurator, null);
            }

            serviceCollection.AddHostedService<MessageBrokerHostedService>();

            return serviceCollection;
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
            return Bus.Factory.CreateUsingInMemory(x =>
            {
                x.TransportConcurrencyLimit = 100;
            });
        }
    }
}