using System;
using MassTransit;
using metrics.Broker.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker.Rabbitmq
{
    internal class HandlerConfigurator : IHandlerConfigurator
    {
        private readonly IBusControl _busControl;
        private readonly IServiceProvider _serviceProvider;

        public HandlerConfigurator(IServiceCollection serviceProvider, IBusControl busControl)
        {
            _serviceProvider = serviceProvider.BuildServiceProvider();
            _busControl = busControl;
        }

        public void ConfigureConsumer<TEvent>() where TEvent : class
        {
            _busControl.ConnectReceiveEndpoint(typeof(TEvent).Name + "-queue", configurator =>
            {
                var createFactory = new MessageHandler<TEvent>(_serviceProvider.GetService<IMessageHandler<TEvent>>());
                configurator.Consumer(typeof(MessageHandler<TEvent>),
                    _ => createFactory);
            });
        }

        public void ConfigureCommandConsumer<TEvent>() where TEvent : class
        {
            _busControl.ConnectReceiveEndpoint(typeof(TEvent).Name, configurator =>
            {
                var createFactory = new MessageHandler<TEvent>(_serviceProvider.GetService<IMessageHandler<TEvent>>());
                configurator.Consumer(typeof(MessageHandler<TEvent>),
                    _ => createFactory);
            });
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