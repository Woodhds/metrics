using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace metrics.Data.Abstractions
{
    public interface ITransactionScopeFactory
    {
        Task<ITransactionRepositoryContext> CreateAsync(IsolationLevel level,
            CancellationToken cancellationToken = default);

        Task<ITransactionRepositoryContext> CreateAsync(CancellationToken ct = default) =>
            CreateAsync(IsolationLevel.ReadCommitted, ct);

        IQueryContext CreateQuery(IsolationLevel level, CancellationToken cancellationToken = default);

        IQueryContext CreateQuery(CancellationToken cancellationToken = default) =>
            CreateQuery(IsolationLevel.ReadCommitted, cancellationToken);

        Task<IBatchTransactionContext> CreateBatchAsync(IsolationLevel level, CancellationToken ct = default);

        Task<IBatchTransactionContext> CreateBatchAsync(CancellationToken ct = default) =>
            CreateBatchAsync(IsolationLevel.ReadCommitted, ct);
    }
}