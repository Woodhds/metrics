using System;
using System.Threading;
using System.Threading.Tasks;

namespace metrics.Cache.Abstractions
{
    public interface ICachingService
    {
        Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);

        Task SetAsync<T>(string key, T obj, CancellationToken cancellationToken = default) =>
            SetAsync(key, obj, TimeSpan.MaxValue, cancellationToken);
        Task SetAsync<T>(string key, T obj, TimeSpan duration, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    }
}