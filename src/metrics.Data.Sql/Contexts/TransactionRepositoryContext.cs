using System;
using System.Threading;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace metrics.Data.Sql.Contexts
{
    public sealed class TransactionRepositoryContext : QueryContext, ITransactionRepositoryContext
    {
        private readonly ITransactionContext _transactionContext;
        private readonly DbContext _context;
        private bool _disposed;

        public TransactionRepositoryContext(DbContext context, ITransactionContext transactionContext) : base(context)
        {
            _context = context;
            _transactionContext = transactionContext;
        }

        public Task CommitAsync(CancellationToken cancellationToken = default) =>
            _transactionContext.CommitAsync(cancellationToken);

        public Task RollbackAsync(CancellationToken cancellationToken = default) =>
            _transactionContext.CommitAsync(cancellationToken);

        public IRepository<T> GetRepository<T>() where T : class, new()
        {
            return new EFRepository<T>(_context);
        }
        
        ~TransactionRepositoryContext()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _transactionContext?.Dispose();
            }

            _disposed = true;
        }

        public override void Dispose()
        {
            Dispose(true);
            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}