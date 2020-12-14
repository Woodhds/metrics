using System;
using System.Threading;
using System.Threading.Tasks;

namespace metrics.Data.Abstractions
{
    public interface ITransactionContext : IDisposable, IAsyncDisposable
    {
        Task? CommitAsync(CancellationToken cancellationToken = default);
        Task? RollbackAsync(CancellationToken cancellationToken = default);
    }
}