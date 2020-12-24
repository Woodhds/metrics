using System.Data;
using System.Threading;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
using metrics.Data.Sql.Contexts;
using Microsoft.EntityFrameworkCore;

namespace metrics.Data.Sql
{
    public class TransactionScopeFactory<TContext> : ITransactionScopeFactory where TContext : DbContext
    {
        private readonly IDbContextFactory<TContext> _dataContextFactory;

        public TransactionScopeFactory(IDbContextFactory<TContext> dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        public async Task<ITransactionRepositoryContext> CreateAsync(IsolationLevel level = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            var context = _dataContextFactory.CreateDbContext();
            var transaction = await context.Database.BeginTransactionAsync(level, cancellationToken);

            return new TransactionRepositoryContext(context, new TransactionContext(transaction));
        }

        public IQueryContext CreateQuery(IsolationLevel level = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            var context = _dataContextFactory.CreateDbContext();

            return new QueryContext(context);
        }

        public async Task<IBatchTransactionContext> CreateBatchAsync(IsolationLevel level, CancellationToken ct = default)
        {
            var context = _dataContextFactory.CreateDbContext();
            
            return new BatchTransactionContext(await context.Database.BeginTransactionAsync(ct), context); 
        }

        public async Task<IRawSqlTransactionContext> CreateRawAsync(IsolationLevel level, CancellationToken ct = default)
        {
            var context = _dataContextFactory.CreateDbContext();
            
            return new RawSqlTransactionContext(context, await context.Database.BeginTransactionAsync(ct)); 
        }
    }
}