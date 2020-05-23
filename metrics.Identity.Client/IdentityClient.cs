using System.Net.Http;
using System.Threading.Tasks;
using Base.Contracts.Options;
using metrics.Identity.Client.Abstractions;
using Microsoft.Extensions.Options;

namespace metrics.Identity.Client
{
    public class IdentityClient : IIdentityClient
    {
        private readonly ISystemTokenGenerationService _jsonWebTokenGenerationService;
        private readonly IOptionsMonitor<IdentityOptions> _options;
        private readonly HttpClient _httpClient;

        public IdentityClient(ISystemTokenGenerationService jsonWebTokenGenerationService,
            IOptionsMonitor<IdentityOptions> options, IHttpClientFactory httpClientFactory)
        {
            _jsonWebTokenGenerationService = jsonWebTokenGenerationService;
            _options = options;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<string> GetToken(int userId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                _options.CurrentValue.UserToken + "/" + userId);
            request.Headers.Add("Authorization", "Bearer " + _jsonWebTokenGenerationService.GetSystemToken());

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}