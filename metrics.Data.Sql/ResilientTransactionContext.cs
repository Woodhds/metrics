using System;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace metrics.Data.Sql
{
    public class ResilientTransactionContext: IResilientTransactionContext
    {
        private readonly ITransactionRepositoryContext _transactionContext;
        private readonly IExecutionStrategy _executionStrategy;
        private bool _disposed;

        public ResilientTransactionContext(
            ITransactionRepositoryContext transactionContext,
            IExecutionStrategy executionStrategy
        )
        {
            _transactionContext = transactionContext;
            _executionStrategy = executionStrategy;
        }

        public Task ExecuteAsync(Func<ITransactionRepositoryContext, Task> context)
        {
            return _executionStrategy.ExecuteAsync(_transactionContext, context.Invoke);
        }

        ~ResilientTransactionContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool dispose)
        {
            if (_disposed)
                return;

            if (dispose)
            {
                _transactionContext?.Dispose();
            }

            _disposed = true;
        }
    }
}