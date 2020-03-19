using System;
using MassTransit;
using metrics.Broker.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker
{
    public class HandlerConfigurator : IHandlerConfigurator
    {
        private readonly IReceiveEndpointConfigurator _configurator;
        private readonly IServiceProvider _serviceProvider;

        public HandlerConfigurator(IReceiveEndpointConfigurator configurator, IServiceProvider serviceProvider)
        {
            _configurator = configurator;
            _serviceProvider = serviceProvider;
        }

        public void Configure<TEvent>() where TEvent : class
        {
            var createFactory = new MessageHandler<TEvent>(_serviceProvider.GetService<IMessageHandler<TEvent>>());
            _configurator.Consumer(typeof(MessageHandler<TEvent>),
                _ => createFactory);
        }
    }
}