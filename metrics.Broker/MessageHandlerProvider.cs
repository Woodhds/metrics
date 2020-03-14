using System.Collections.Generic;
using System.Linq;
using metrics.Broker.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker
{
    public class MessageHandlerProvider : IMessageHandlerProvider
    {
        private readonly IServiceCollection _serviceCollection;
        public MessageHandlerProvider(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public void Register<T>() where T : class, IMessageHandler<T>
        {
            _serviceCollection.AddSingleton<T>();
        }

        public IEnumerable<IMessageHandler> GetAll()
        {
            return _serviceCollection.Where(f => typeof(IMessageHandler<>).IsAssignableFrom(f.ImplementationType)).Cast<IMessageHandler>();
        }
    }
}