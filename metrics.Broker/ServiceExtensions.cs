using System;
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
            Func<IServiceCollection, IConfiguration, IBrokerConfigurationBuilder> configurator,
            Action<IMessageHandlerProvider>? handlerProvider = null
        )
        {
            var hp = new MessageHandlerProvider(serviceCollection);

            handlerProvider?.Invoke(hp);
            
            var builder = configurator(serviceCollection, configuration);
            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            
            var messageBrokerConfig = builder.Build(serviceProvider);

            var handlerConfigurator = messageBrokerConfig.HandlerConfigurator;
            
            foreach (var (tEvent, _) in hp.GetConsumers())
            {
                var method = typeof(IHandlerConfigurator).GetMethod(nameof(IHandlerConfigurator.ConfigureConsumer))
                    ?.MakeGenericMethod(tEvent);
                method?.Invoke(handlerConfigurator, null);
            }

            foreach (var command in hp.GetCommands())
            {
                var method = typeof(IHandlerConfigurator).GetMethod(nameof(IHandlerConfigurator.ConfigureCommand))
                    ?.MakeGenericMethod(command);
                method?.Invoke(handlerConfigurator, new object[]{messageBrokerConfig.Host});
            }
            
            foreach (var (tEvent, _) in hp.GetCommandConsumers())
            {
                var method = typeof(IHandlerConfigurator).GetMethod(nameof(IHandlerConfigurator.ConfigureCommandConsumer))
                    ?.MakeGenericMethod(tEvent);
                method?.Invoke(handlerConfigurator, null);
            }

            return serviceCollection;
        }
    }
}