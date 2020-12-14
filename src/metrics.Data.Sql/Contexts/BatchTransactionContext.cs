using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace metrics.Data.Sql.Contexts
{
    public class BatchTransactionContext : QueryContext, IBatchTransactionContext
    {
        private IDbContextTransaction? _dbContextTransaction;
        private DbContext? _dbContext;

        public BatchTransactionContext(IDbContextTransaction dbContextTransaction, DbContext dbContext) :
            base(dbContext)
        {
            _dbContextTransaction = dbContextTransaction ?? throw new ArgumentNullException(nameof(dbContextTransaction));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task? CreateCollectionAsync<T>(IEnumerable<T> collection, CancellationToken ct = default) where T : class
        {
            return _dbContext?.Set<T>().AddRangeAsync(collection, ct);
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            
            _dbContextTransaction?.Dispose();
            _dbContext?.Dispose();
            _dbContext = null;
            _dbContextTransaction = null;
            base.Dispose();
        }

        public override async ValueTask DisposeAsync()
        {
            if (_dbContextTransaction != null)
            {
                await _dbContextTransaction.DisposeAsync().ConfigureAwait(false);
                
            }
            _dbContextTransaction = null;
            Dispose(false);
            await base.DisposeAsync();
            
            GC.SuppressFinalize(this);
        }

        public async Task? CommitAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext?.SaveChangesAsync(cancellationToken)!;
            await _dbContextTransaction?.CommitAsync(cancellationToken)!;
        }

        public Task? RollbackAsync(CancellationToken cancellationToken = default)
        {
            return _dbContextTransaction?.RollbackAsync(cancellationToken);
        }
    }
}