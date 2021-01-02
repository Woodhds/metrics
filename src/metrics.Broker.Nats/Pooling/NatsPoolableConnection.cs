using System;
using NATS.Client;

namespace metrics.Broker.Nats.Pooling
{
    public interface INatsPoolableConnection : INatsConnection
    {
    }
    
    public class NatsPoolableConnection : INatsPoolableConnection
    {
        private INatsPool _pool;
        private IConnection _connection;

        public NatsPoolableConnection(INatsPool pool, Action<Options> optionsAction = null)
        {
            _pool = pool;
            var options = ConnectionFactory.GetDefaultOptions();
            optionsAction?.Invoke(options);
            _connection = new ConnectionFactory().CreateConnection(options);
        }

        public void Dispose()
        {
            if (!_pool.IsActive)
            {
                _connection.Dispose();
                _connection = null;
                return;
            }
            
            _pool.Return(this);
        }

        public void Publish(string subject, byte[] bytes)
        {
            _connection.Publish(subject, bytes);
        }

        public IAsyncSubscription SubscribeAsync(string subject, EventHandler<MsgHandlerEventArgs> handler)
        {
            return _connection.SubscribeAsync(subject, handler);
        }
    }
}