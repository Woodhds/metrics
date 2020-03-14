using System;
using System.Threading;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore.Storage;

namespace metrics.Data.Sql
{
    public sealed class TransactionContext : ITransactionContext
    {
        private readonly IDbContextTransaction _dbContextTransaction;
        private bool _disposed;

        public TransactionContext(IDbContextTransaction dbContextTransaction)
        {
            _dbContextTransaction = dbContextTransaction;
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return _dbContextTransaction?.CommitAsync(cancellationToken);
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            return _dbContextTransaction?.RollbackAsync(cancellationToken);
        }

        ~TransactionContext()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _dbContextTransaction?.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}