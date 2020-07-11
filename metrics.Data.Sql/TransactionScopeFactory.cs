using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

//TODO: DRY
namespace metrics.Data.Sql
{
    public class TransactionScopeFactory : ITransactionScopeFactory
    {
        private readonly IDataContextFactory _dataContextFactory;

        public TransactionScopeFactory(IDataContextFactory dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        public async Task<ITransactionContext> CreateAsync(IsolationLevel level = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            var context = _dataContextFactory.Create();
            var transaction = await context.Database.BeginTransactionAsync(level, cancellationToken);

            return new TransactionContext(transaction, context);
        }

        public IQueryContext CreateQuery(IsolationLevel level = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
        {
            var context = _dataContextFactory.Create();

            return new QueryContext(context);
        }

        public Task<ITransactionContext> CreateResilientAsync(IsolationLevel level = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
           throw new NotImplementedException();
        }
    }
}