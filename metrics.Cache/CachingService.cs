using System.Threading;
using System.Threading.Tasks;
using metrics.Cache.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace metrics.Cache
{
    public class CachingService : ICachingService
    {
        private readonly IDistributedCache _cache;
        private readonly ICachingSerializer _serializer;

        public CachingService(IDistributedCache cache, ICachingSerializer serializer)
        {
            _cache = cache;
            _serializer = serializer;
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var bytes = await _cache.GetAsync(key, cancellationToken);
            return _serializer.Deserialize<T>(bytes);
        }

        public async Task SetAsync<T>(string key, T obj, CancellationToken cancellationToken = default)
        {
            await _cache.SetAsync(key, _serializer.Serialize(obj), cancellationToken);
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            return _cache.RemoveAsync(key, cancellationToken);
        }
    }
}