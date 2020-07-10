using System.Threading;
using System.Threading.Tasks;

namespace metrics.Data.Abstractions
{
    public interface ITransactionContext : IQueryContext
    {
        IRepository<T> GetRepository<T>() where T : class, new();
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}