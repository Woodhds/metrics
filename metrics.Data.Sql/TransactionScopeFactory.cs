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

        public async Task<ITransactionContext> CreateAsync(IsolationLevel level = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            var transaction =
                await _dataContextFactory.Create().Database.BeginTransactionAsync(level, cancellationToken);
            
            return new TransactionContext(transaction);
        }
    }
}