using System;
using System.Collections.Generic;
using System.Linq;
using metrics.Broker.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker
{
    public class MessageHandlerProvider : IMessageHandlerProvider
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly Dictionary<Type, Type> _types = new Dictionary<Type, Type>();
        public MessageHandlerProvider(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public void Register<TEvent, THandler>() where THandler : class, IMessageHandler<TEvent> where TEvent : class
        {
            _serviceCollection.AddScoped<IMessageHandler<TEvent>, THandler>();
            if (!_types.ContainsKey(typeof(TEvent)))
            {
                _types.Add(typeof(TEvent), typeof(THandler));
            }
        }

        public IEnumerable<(Type, Type)> GetTypes()
        {
            return _types.Select(f => (f.Key, f.Value));
        }
    }
}