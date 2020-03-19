using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using metrics.Data.Abstractions;
using metrics.Data.Common.Infrastructure.Entities;
using metrics.Services.Abstractions;

namespace metrics.Broker.Console
{
    public class CacheTokenAccessor : IVkTokenAccessor
    {
        private readonly ConcurrentDictionary<int, string> _tokens = new ConcurrentDictionary<int, string>();
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        public CacheTokenAccessor(ITransactionScopeFactory transactionScopeFactory)
        {
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task<string> GetTokenAsync(int? userId = null)
        {
            if (!userId.HasValue)
            {
                return string.Empty;
            }

            return _tokens.GetOrAdd(userId.Value, _ =>
            {
                using var scope = _transactionScopeFactory.Create();
                
                return scope.GetRepository<UserToken>()
                    .Read()
                    .Where(f => f.UserId == userId.Value)
                    .Select(f => f.Token)
                    .FirstOrDefault();
            });
        }
    }
}