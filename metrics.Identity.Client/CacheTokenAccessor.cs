using System.Threading.Tasks;
using metrics.Cache.Abstractions;
using metrics.Data.Abstractions;
using metrics.Identity.Client.Abstractions;
using metrics.Services.Abstractions;

namespace metrics.Identity.Client
{
    public class CacheTokenAccessor : IVkTokenAccessor
    {
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly ICachingService _cachingService;
        private readonly IIdentityClient _identityClient;

        public CacheTokenAccessor(ITransactionScopeFactory transactionScopeFactory, ICachingService cachingService, IIdentityClient identityClient)
        {
            _transactionScopeFactory = transactionScopeFactory;
            _cachingService = cachingService;
            _identityClient = identityClient;
        }

        public async Task<string> GetTokenAsync(int? userId = null)
        {
            if (!userId.HasValue)
                return string.Empty;
            
            var token = await _cachingService.GetAsync<string>(userId.ToString());

            if (string.IsNullOrEmpty(token))
            {
                using var scope = await _transactionScopeFactory.CreateAsync();
                token = await _identityClient.GetToken(userId.Value);

                if (!string.IsNullOrEmpty(token))
                    await _cachingService.SetAsync(userId.ToString(), token);
            }

            return token;
        }
    }
}