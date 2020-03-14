using System;
using System.Collections.Generic;

namespace metrics.Broker.Abstractions
{
    public interface IMessageHandlerProvider
    {
        void Register<TEvent, THandler>() where THandler : class, IMessageHandler<TEvent> where TEvent : class;
        IEnumerable<(Type, Type)> GetTypes();
    }
}