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

            var bus = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.Host(new Uri($"rabbitmq://{options.Host}"), configurator =>
                {
                    configurator.Username(options.UserName);
                    configurator.Password(options.Password);
                });
            });

            serviceCollection.AddSingleton(bus);
            serviceCollection.AddSingleton<IMessageBroker, MessageBroker>(_ => new MessageBroker(bus));

            var hp = new MessageHandlerProvider(serviceCollection);

            handlerProvider?.Invoke(hp);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            bus.ConnectReceiveEndpoint(configuration["Queue"], configurator =>
            {
                var handlerConfigurator = new HandlerConfigurator(configurator, serviceProvider);
                
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
            });

            bus.Start();

            return serviceCollection;
        }
    }
}