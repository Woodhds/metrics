using System.Threading.Tasks;
using metrics.Authentication.Infrastructure;
using metrics.Cache.Abstractions;
using metrics.Identity.Client.Abstractions;

namespace metrics.Identity.Client
{
    public class CachedUserTokenAccessor : ICacheTokenAccessor
    {
        private readonly ICachingService _cachingService;
        private readonly IdentityTokenService.IdentityTokenServiceClient _identityClient;
        private readonly IAuthenticatedUserProvider _authenticatedUserProvider;

        public CachedUserTokenAccessor(ICachingService cachingService,
            IdentityTokenService.IdentityTokenServiceClient identityClient,
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
                token = (await _identityClient.GetTokenAsync(new IdentityTokenServiceRequest {UserId = user.Id})
                    .ResponseAsync)?.Token;

                if (!string.IsNullOrEmpty(token))
                    await _cachingService.SetAsync(user.Id.ToString(), token);
            }

            return token;
        }
    }
}