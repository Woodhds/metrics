using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Base.Contracts.Options;
using metrics.Authentication.Infrastructure;
using metrics.core.DistributedLock;
using metrics.Diagnostics;
using metrics.Services.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace metrics.Services.Concrete
{
    public class VkClientHttpHandler : DelegatingHandler
    {
        private readonly IOptions<VkontakteOptions> _vkontakteOptions;
        private readonly IUserTokenAccessor _vkTokenAccessor;
        private readonly IDistributedLock _distributedLock;
        private readonly IAuthenticatedUserProvider _authenticatedUserProvider;
        private DiagnosticSource DiagnosticSource { get; } = new DiagnosticListener("VkClientHttpHandler");

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

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            var url = QueryHelpers.AddQueryString(request.RequestUri.ToString(),
                new List<KeyValuePair<string, StringValues>>
                {
                    new("v", _vkontakteOptions.Value.ApiVersion),
                    new("access_token", await _vkTokenAccessor.GetTokenAsync())
                });
            request.RequestUri = new Uri(url);
            using (DiagnosticSource.Diagnose("VkRequest",
                new VkRequestState(_authenticatedUserProvider, url)))
            {
                await using (await _distributedLock.AcquireAsync(_authenticatedUserProvider.GetUser().Id.ToString()))
                {
                    return await base.SendAsync(request, cancellationToken);
                }
            }
        }
    }
}