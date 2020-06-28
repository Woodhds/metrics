using System;
using MassTransit;
using metrics.Broker.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker
{
    public class HandlerConfigurator
    {
        private readonly IReceiveEndpointConfigurator _configurator;
        private readonly IServiceProvider _serviceProvider;

        public HandlerConfigurator(IReceiveEndpointConfigurator configurator, IServiceProvider serviceProvider)
        {
            _configurator = configurator;
            _serviceProvider = serviceProvider;
        }

        public void ConfigureConsumer<TEvent>() where TEvent : class
        {
            var createFactory = new MessageHandler<TEvent>(_serviceProvider.GetService<IMessageHandler<TEvent>>());
            _configurator.Consumer(typeof(MessageHandler<TEvent>),
                _ => createFactory);
        }

        public void ConfigureCommand<TCommand>(string host) where TCommand: class
        {
            if (Uri.TryCreate(host + "/" + typeof(TCommand).Name, UriKind.Absolute, out var uri))
            {
                EndpointConvention.Map<TCommand>(uri);
            }
            else
            {
                Console.WriteLine("ERROR: Invalid uri for command " + typeof(TCommand));
            }
        }
    }
}