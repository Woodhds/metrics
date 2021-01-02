using System;
using NATS.Client;

namespace metrics.Broker.Nats
{
    public interface INatsConnection : IDisposable
    {
        void Publish(string subject, byte[] bytes);
        IAsyncSubscription SubscribeAsync(string subject, EventHandler<MsgHandlerEventArgs> handler);
    }
}