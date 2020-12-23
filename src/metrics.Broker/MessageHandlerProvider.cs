using System;
using System.Collections.Generic;
using System.Linq;
using metrics.Broker.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace metrics.Broker
{
    internal class MessageHandlerProvider : IMessageHandlerProvider
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly Dictionary<Type, Type> _consumers = new();
        private readonly Dictionary<Type, Type> _commandConsumers = new();
        private readonly List<Type> _commands = new();
        public MessageHandlerProvider(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public void RegisterConsumer<TEvent, THandler>() where THandler : class, IMessageHandler<TEvent> where TEvent : class, new()
        {
            _serviceCollection.AddSingleton<IMessageHandler<TEvent>, THandler>();
            if (!_consumers.ContainsKey(typeof(TEvent)))
            {
                _consumers.Add(typeof(TEvent), typeof(THandler));
            }
        }

        public void RegisterCommandConsumer<TEvent, THandler>() where THandler : class, IMessageHandler<TEvent> where TEvent : class, new()
        {
            _serviceCollection.AddSingleton<IMessageHandler<TEvent>, THandler>();
            if (!_commandConsumers.ContainsKey(typeof(TEvent)))
            {
                _commandConsumers.Add(typeof(TEvent), typeof(THandler));
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

        public IEnumerable<(Type, Type)> GetCommandConsumers()
        {
            return _commandConsumers.Select(f => (f.Key, f.Value));
        }

        public void RegisterCommand<TCommand>() where TCommand : class, new()
        {
            _commands.Add(typeof(TCommand));
        }
    }
}