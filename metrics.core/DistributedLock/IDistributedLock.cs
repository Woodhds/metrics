using System;
using System.Threading.Tasks;

namespace metrics.core.DistributedLock
{
    public interface IDistributedLock
    {
        Task<IAsyncDisposable> AcquireAsync(string? key);
    }
}