using System.Threading.Tasks;
using metrics.Authentication;
using metrics.Authentication.Infrastructure;
using metrics.Cache.Abstractions;
using metrics.Identity.Client.Abstractions;

namespace metrics.Identity.Client
{
    public class CacheTokenAccessor : ICacheTokenAccessor
    {
        private readonly ICachingService _cachingService;
        private readonly IIdentityClient _identityClient;
        private readonly IAuthenticatedUserProvider _authenticatedUserProvider;

        public CacheTokenAccessor(ICachingService cachingService, IIdentityClient identityClient,
            IAuthenticatedUserProvider authenticatedUserProvider)
        {
            _cachingService = cachingService;
            _identityClient = identityClient;
            _authenticatedUserProvider = authenticatedUserProvider;
        }

        public async Task<string> GetTokenAsync()
        {
            var user = _authenticatedUserProvider.GetUser();
            
            if (user == null)
                return string.Empty;

            var token = await _cachingService.GetAsync<string>(user.Id.ToString());

            if (string.IsNullOrEmpty(token))
            {
                token = await _identityClient.GetToken(user.Id);

                if (!string.IsNullOrEmpty(token))
                    await _cachingService.SetAsync(user.Id.ToString(), token);
            }

            return token;
        }
    }
}