using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace metrics.Data.Abstractions
{
    public interface ITransactionScopeFactory
    {
        ITransactionContext Create(IsolationLevel level = IsolationLevel.ReadCommitted);
        Task<ITransactionContext> CreateAsync(
            IsolationLevel level = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default
        );
    }
}