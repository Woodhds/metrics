using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Base.Contracts;

namespace metrics.Broker.Console
{
    public interface IRepostCacheAccessor
    {
        IEnumerable<(int userId, VkRepostViewModel repost)> Get();
        void Set(int userId, IEnumerable<VkRepostViewModel> models);
    }

    public class RepostCacheAccessor : IRepostCacheAccessor
    {
        private readonly ConcurrentDictionary<int, List<VkRepostViewModel>> _cache =
            new ConcurrentDictionary<int, List<VkRepostViewModel>>();

        public IEnumerable<(int userId, VkRepostViewModel repost)> Get()
        {
            var obj = _cache.Keys.Select(key =>
            (
                key,
                _cache[key].FirstOrDefault()
            )).ToList();

            obj.ForEach(z => { _cache[z.key].Remove(z.Item2); });

            return obj;
        }

        public void Set(int userId, IEnumerable<VkRepostViewModel> models)
        {
            _cache.AddOrUpdate(
                userId,
                key => models.ToList(),
                (i, tuple) => tuple.Concat(models).Distinct().ToList()
            );
        }
    }
}