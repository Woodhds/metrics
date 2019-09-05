using System.Collections.Concurrent;
using metrics.Services.Abstract;

namespace metrics.Services.Concrete
{
    public class EventStorage: IEventStorage
    {
        private ConcurrentDictionary<string, int> _users = new ConcurrentDictionary<string, int>();
        
        public int AddEvents(string userId, int count)
        {
            _users.AddOrUpdate(userId, count, (s, i) => i + count);
            return GetCurrentCount(userId);
        }

        public int GetCurrentCount(string userId)
        {
            if (!string.IsNullOrEmpty(userId) && _users.TryGetValue(userId, out var count))
            {
                return count;
            }

            return 0;
        }
    }
}