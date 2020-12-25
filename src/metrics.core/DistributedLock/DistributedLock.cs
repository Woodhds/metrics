using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace metrics.core.DistributedLock
{
    public class DistributedLock : IDistributedLock
    {
        private readonly IDatabase _database;

        private class LockItem : IAsyncDisposable
        {
            private readonly RedisKey _key;
            private readonly RedisValue _value;
            private readonly IDatabase _database;

            public LockItem(IDatabase database, RedisKey key, RedisValue value)
            {
                _key = key;
                _database = database;
                _value = value;
            }

            public async ValueTask DisposeAsync()
            {
                await _database.LockReleaseAsync(_key, _value).ConfigureAwait(false);
            }
        }

        public DistributedLock(IDatabase database)
        {
            _database = database;
        }

        public async Task<IAsyncDisposable> AcquireAsync(string? key)
        {
            while (!await _database.LockTakeAsync(key, key, TimeSpan.FromSeconds(30)))
            {
                await Task.Delay(300);
            }

            return new LockItem(_database, key, key);
        }
    }
}