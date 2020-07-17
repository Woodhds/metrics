using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace metrics.Data.Sql
{
    public class BatchTransactionContext : QueryContext, IBatchTransactionContext
    {
        private readonly IDbContextTransaction _dbContextTransaction;
        private readonly DbContext _dbContext;
        private bool _disposed;

        public BatchTransactionContext(IDbContextTransaction dbContextTransaction, DbContext dbContext) :
            base(dbContext)
        {
            _dbContextTransaction = dbContextTransaction;
            _dbContext = dbContext;
        }

        public Task CreateCollectionAsync<T>(IEnumerable<T> collection, CancellationToken ct = default) where T : class
        {
            return _dbContext.Set<T>().AddRangeAsync(collection, ct);
        }


        ~BatchTransactionContext()
        {
            Dispose(false);
        }
        public override void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (_disposed || !disposing) return;
            
            _dbContextTransaction?.Dispose();
            _dbContext?.Dispose();
            base.Dispose();

            _disposed = true;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbContextTransaction.CommitAsync(cancellationToken);
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            return _dbContextTransaction.RollbackAsync(cancellationToken);
        }
    }
}