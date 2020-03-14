using System.Collections.Generic;

namespace metrics.Broker.Abstractions
{
    public interface IMessageHandlerProvider
    {
        void Register<T>() where T: class, IMessageHandler<T>;
        IEnumerable<IMessageHandler> GetAll();
    }
}