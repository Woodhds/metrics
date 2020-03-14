using System;
using MassTransit;
using metrics.Broker.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMessageBroker(
            this IServiceCollection serviceCollection, Action<IMessageHandlerProvider> handlerProvider = null
        )
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.Host(new Uri("rabbitmq://192.168.99.100:30672"), configurator =>
                {
                    configurator.Username("user");
                    configurator.Password("password");
                });
            });

            serviceCollection.AddSingleton<IMessageBroker, MessageBroker>(_ => new MessageBroker(bus));
            
            var hp = new MessageHandlerProvider(serviceCollection);
            handlerProvider?.Invoke(hp);
            bus.ConnectReceiveEndpoint("q", endpointConfigurator =>
            {
                foreach (var handler in hp.GetAll())
                {
                    Console.WriteLine(handler);
                }

            });

            bus.Start();

            return serviceCollection;
        }
    }
}