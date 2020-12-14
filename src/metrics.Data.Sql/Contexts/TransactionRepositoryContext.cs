using System;
using System.Threading;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace metrics.Data.Sql.Contexts
{
    public sealed class TransactionRepositoryContext : QueryContext, ITransactionRepositoryContext
    {
        private ITransactionContext? _transactionContext;
        private DbContext? _context;

        public TransactionRepositoryContext(DbContext context, ITransactionContext transactionContext) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _transactionContext = transactionContext ?? throw new ArgumentNullException(nameof(transactionContext));
        }

        public Task? CommitAsync(CancellationToken cancellationToken = default) =>
            _transactionContext?.CommitAsync(cancellationToken);

        public Task? RollbackAsync(CancellationToken cancellationToken = default) =>
            _transactionContext?.CommitAsync(cancellationToken);

        public IRepository<T> GetRepository<T>() where T : class, new()
        {
            return new EFRepository<T>(_context);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _transactionContext?.Dispose();
            }

            _transactionContext = null;
        }

        public override async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            Dispose(false);
            await base.DisposeAsync();
            GC.SuppressFinalize(this);
        }

        public override void Dispose()
        {
            Dispose(true);
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        private async ValueTask DisposeAsyncCore()
        {
            if (_context != null)
            {
                await _context.DisposeAsync().ConfigureAwait(false);
            }

            if (_transactionContext != null)
            {
                await _transactionContext.DisposeAsync().ConfigureAwait(false);
            }

            _context = null;
            _transactionContext = null;
        }
    }
}