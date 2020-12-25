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
        private readonly IUserTokenKeyProvider _userTokenKeyProvider;

        public CachedUserTokenAccessor(
            ICachingService cachingService,
            IdentityTokenService.IdentityTokenServiceClient identityClient,
            IAuthenticatedUserProvider authenticatedUserProvider,
            IUserTokenKeyProvider userTokenKeyProvider
        )
        {
            _cachingService = cachingService;
            _identityClient = identityClient;
            _authenticatedUserProvider = authenticatedUserProvider;
            _userTokenKeyProvider = userTokenKeyProvider;
        }

        public async ValueTask<string> GetTokenAsync()
        {
            var user = _authenticatedUserProvider.GetUser();

            if (user == null)
                return string.Empty;

            var token = await _cachingService.GetAsync<string>(_userTokenKeyProvider.GetKey(user.Id));

            if (string.IsNullOrEmpty(token))
            {
                token = (await _identityClient.GetTokenAsync(new IdentityTokenServiceRequest {UserId = user.Id})
                    .ResponseAsync)?.Token;

                if (!string.IsNullOrEmpty(token))
                    await _cachingService.SetAsync(_userTokenKeyProvider.GetKey(user.Id), token);
            }

            return token;
        }
    }
}