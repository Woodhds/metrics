using System.Data;
using System.Threading;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
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

        public ITransactionContext Create(IsolationLevel level = IsolationLevel.ReadCommitted)
        {
            var context = _dataContextFactory.Create();
            var transaction = context.Database.BeginTransaction(level);
            return new TransactionContext(transaction, context);
        }

        public async Task<ITransactionContext> CreateAsync(IsolationLevel level = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            var context = _dataContextFactory.Create();
            var transaction = await context.Database.BeginTransactionAsync(level, cancellationToken);

            return new TransactionContext(transaction, context);
        }
    }
}