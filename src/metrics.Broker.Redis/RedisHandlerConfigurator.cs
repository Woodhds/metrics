using metrics.Broker.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker.Redis
{
    public class RedisHandlerConfigurator : IHandlerConfigurator
    {
        private readonly IServiceCollection _serviceCollection;
        
        public RedisHandlerConfigurator(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }
        public void ConfigureConsumer<TEvent>() where TEvent : class, new()
        {
            _serviceCollection.AddHostedService<RedisHostedHandler<TEvent>>();
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