using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace metrics.Data.Abstractions
{
    public interface IBatchTransactionContext : ITransactionContext, IQueryContext
    {
        Task CreateCollectionAsync<T>(IEnumerable<T> collection, CancellationToken ct = default) where T: class;
    }
}