using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts.Options;
using metrics.Authentication.Infrastructure;
using metrics.core.DistributedLock;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace metrics.Services.Concrete
{
    public class VkClientHttpHandler : DelegatingHandler
    {
        private readonly IOptions<VkontakteOptions> _vkontakteOptions;
        private readonly IUserTokenAccessor _vkTokenAccessor;
        private readonly IDistributedLock _distributedLock;
        private readonly IAuthenticatedUserProvider _authenticatedUserProvider;

        public VkClientHttpHandler(
            IOptions<VkontakteOptions> vkontakteOptions,
            IUserTokenAccessor vkTokenAccessor,
            IDistributedLock distributedLock,
            IAuthenticatedUserProvider authenticatedUserProvider
        )
        {
            _vkontakteOptions = vkontakteOptions;
            _vkTokenAccessor = vkTokenAccessor;
            _distributedLock = distributedLock;
            _authenticatedUserProvider = authenticatedUserProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var url = QueryHelpers.AddQueryString(request.RequestUri.ToString(), "v",
                _vkontakteOptions.Value.ApiVersion);
            url = QueryHelpers.AddQueryString(url, "access_token", await _vkTokenAccessor.GetTokenAsync());
            request.RequestUri = new Uri(url);
            await using (await _distributedLock.AcquireAsync(_authenticatedUserProvider.GetUser().Id.ToString()))
            {
                return await base.SendAsync(request, cancellationToken);
            }
        }
    }
}