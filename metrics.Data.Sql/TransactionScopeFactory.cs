using System.Data;
using System.Threading;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
using metrics.Data.Sql.Contexts;
using Microsoft.EntityFrameworkCore;

namespace metrics.Data.Sql
{
    public class TransactionScopeFactory : ITransactionScopeFactory
    {
        private readonly IDataContextFactory _dataContextFactory;

        public TransactionScopeFactory(IDataContextFactory dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        public async Task<ITransactionRepositoryContext> CreateAsync(IsolationLevel level = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            var context = _dataContextFactory.Create();
            var transaction = await context.Database.BeginTransactionAsync(level, cancellationToken);

            return new TransactionRepositoryContext(context, new TransactionContext(transaction));
        }

        public IQueryContext CreateQuery(IsolationLevel level = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            var context = _dataContextFactory.Create();

            return new QueryContext(context);
        }

        public async Task<IBatchTransactionContext> CreateBatchAsync(IsolationLevel level, CancellationToken ct = default)
        {
            var context = _dataContextFactory.Create();
            
            return new BatchTransactionContext(await context.Database.BeginTransactionAsync(ct), context); 
        }
    }
}