using System;
using System.Threading;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore.Storage;

namespace metrics.Data.Sql.Contexts
{
    public class TransactionContext : ITransactionContext
    {
        private IDbContextTransaction? _dbContextTransaction;

        public TransactionContext(IDbContextTransaction dbContextTransaction)
        {
            _dbContextTransaction = dbContextTransaction ?? throw new ArgumentNullException(nameof(dbContextTransaction));
        }

        public Task? CommitAsync(CancellationToken cancellationToken = default)
        {
            return _dbContextTransaction?.CommitAsync(cancellationToken);
        }

        public Task? RollbackAsync(CancellationToken cancellationToken = default)
        {
            return _dbContextTransaction?.RollbackAsync(cancellationToken);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContextTransaction?.Dispose();
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        private async ValueTask DisposeAsyncCore()
        {
            if (_dbContextTransaction != null)
            {
                await _dbContextTransaction.DisposeAsync().ConfigureAwait(false);
            }
           
            _dbContextTransaction = null;
        }
    }
}