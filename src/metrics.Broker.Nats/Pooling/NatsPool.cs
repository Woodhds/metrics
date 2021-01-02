using System;
using System.Collections.Concurrent;
using System.Threading;
using NATS.Client;

namespace metrics.Broker.Nats.Pooling
{
    public interface INatsPool : IDisposable
    {
        bool IsActive { get; }
        INatsPoolableConnection Rent();
        void Return(INatsPoolableConnection connection);
    }

    public class NatsPool : INatsPool
    {
        private int _count;
        private int _maxSize;
        private static readonly int DefaultPoolSize = 32;
        private readonly ConcurrentQueue<INatsPoolableConnection> _pool = new();
        private readonly Action<Options> _actionOption;
        public bool IsActive { get; private set; }

        public NatsPool(int? poolSize = null, Action<Options> options = null)
        {
            _maxSize = poolSize ?? DefaultPoolSize;
            _actionOption = options;
            IsActive = true;
        }

        public INatsPoolableConnection Rent()
        {
            if (_pool.TryDequeue(out var poolable))
            {
                Interlocked.Decrement(ref _count);

                return poolable;
            }

            return new NatsPoolableConnection(this, _actionOption);
        }

        public void Return(INatsPoolableConnection poolable)
        {
            if (Interlocked.Increment(ref _count) <= _maxSize)
            {
                _pool.Enqueue(poolable);
            }
            else
            {
                Interlocked.Decrement(ref _count);
                poolable.Dispose();
            }
        }

        public void Dispose()
        {
            _maxSize = 0;
            IsActive = false;
            while (_pool.TryDequeue(out var poolable))
            {
                poolable.Dispose();
            }
        }
    }
}