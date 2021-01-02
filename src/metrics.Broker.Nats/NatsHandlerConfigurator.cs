using metrics.Broker.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker.Nats
{
    public class NatsHandlerConfigurator : IHandlerConfigurator
    {
        private readonly IServiceCollection _serviceCollection;

        public NatsHandlerConfigurator(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public void ConfigureConsumer<TEvent>() where TEvent : class, new()
        {
            _serviceCollection.AddHostedService<NatsHostedHandler<TEvent>>();
        }

        public void ConfigureCommandConsumer<TEvent>() where TEvent : class, new()
        {
            ConfigureConsumer<TEvent>();
        }

        public void ConfigureCommand<TCommand>(string host) where TCommand : class, new()
        {
        }
    }
}