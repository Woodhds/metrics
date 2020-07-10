using System;
using System.Threading;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace metrics.Data.Sql
{
    public sealed class TransactionContext : QueryContext, ITransactionContext
    {
        private readonly IDbContextTransaction _dbContextTransaction;
        private readonly DbContext _dbContext;
        private bool _disposed;

        public TransactionContext(
            IDbContextTransaction dbContextTransaction,
            DbContext dbContext
        ) : base(dbContext)
        {
            _dbContextTransaction = dbContextTransaction;
            _dbContext = dbContext;
        }

        public IRepository<T> GetRepository<T>() where T : class, new()
        {
            return new EFRepository<T>(_dbContext);
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
                _dbContext?.Dispose();
                _dbContextTransaction?.Dispose();
            }

            _disposed = true;
        }

        public override void Dispose()
        {
            base.Dispose();
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}