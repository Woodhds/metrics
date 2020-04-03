using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Base.Contracts;
using metrics.Cache.Abstractions;

namespace metrics.Services.Concrete
{
    public interface IRepostCacheAccessor
    {
        Task<IEnumerable<(int userId, VkRepostViewModel repost)>> GetAsync();
        Task SetAsync(int userId, IEnumerable<VkRepostViewModel> models);

        ValueTask<int> GetCountAsync(int userId);
    }

    public class RepostCacheAccessor : IRepostCacheAccessor
    {
        private readonly ICachingService _cache;

        public RepostCacheAccessor(ICachingService cachingService)
        {
            _cache = cachingService;
        }

        public async Task<IEnumerable<(int userId, VkRepostViewModel repost)>> GetAsync()
        {
            var keys = await _cache.GetAsync<HashSet<int>>("queue");

            var obj = new List<(int key, List<VkRepostViewModel>)>();
            foreach (var key in keys)
            {
                obj.Add((key, await _cache.GetAsync<List<VkRepostViewModel>>(key.ToString())));
            }
            
            var result = new List<(int userId, VkRepostViewModel)>();

            async Task RemoveKey(int key)
            {
                await _cache.RemoveAsync(key.ToString());
                keys.Remove(key);
                await _cache.SetAsync("queue", keys);
            }
            
            obj.ForEach(async z =>
            {
                if (z.Item2 == null)
                {
                    await RemoveKey(z.key);
                    return;
                }

                var item = z.Item2.FirstOrDefault();

                result.Add((z.key, item));
            });

            return result;
        }

        public async Task SetAsync(int userId, IEnumerable<VkRepostViewModel> models)
        {
            var list = await _cache.GetAsync<List<VkRepostViewModel>>(userId.ToString());
            var users = await _cache.GetAsync<HashSet<int>>("queue");
            users?.Add(userId);
            await _cache.SetAsync("queue", users != null ? users.Distinct() : new[] {userId});

            await _cache.SetAsync(userId.ToString(),
                list != null
                    ? list.Concat(models).Distinct()
                    : models);
        }

        public async ValueTask<int> GetCountAsync(int userId)
        {
            return (await _cache.GetAsync<List<VkRepostViewModel>>(userId.ToString()))?.Count ?? 0;
        }
    }
}