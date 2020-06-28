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
        private readonly Dictionary<Type, Type> _consumers = new Dictionary<Type, Type>();
        private readonly List<Type> _commands = new List<Type>();
        public MessageHandlerProvider(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public void RegisterConsumer<TEvent, THandler>() where THandler : class, IMessageHandler<TEvent> where TEvent : class
        {
            _serviceCollection.AddScoped<IMessageHandler<TEvent>, THandler>();
            if (!_consumers.ContainsKey(typeof(TEvent)))
            {
                _consumers.Add(typeof(TEvent), typeof(THandler));
            }
        }

        public IEnumerable<(Type, Type)> GetConsumers()
        {
            return _consumers.Select(f => (f.Key, f.Value));
        }

        public IEnumerable<Type> GetCommands()
        {
            return _commands;
        }

        public void RegisterCommand<TCommand>() where TCommand : class
        {
            _commands.Add(typeof(TCommand));
        }
    }
}