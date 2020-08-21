using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using metrics.Serialization.Abstractions;
using metrics.Services.Utils.Helpers;
using Microsoft.Extensions.Logging;

namespace metrics.Services.Utils
{
    public class BaseHttpClient
    {
        private readonly HttpClient _httpClient;
        protected readonly ILogger<BaseHttpClient> Logger;
        private readonly IJsonSerializer _jsonSerializer;

        public BaseHttpClient(
            IHttpClientFactory httpClientFactory,
            ILogger<BaseHttpClient> logger, IJsonSerializer jsonSerializer)
        {
            Logger = logger;
            _jsonSerializer = jsonSerializer;
            _httpClient = httpClientFactory.CreateClient();
        }

        protected virtual async Task<T> PostAsync<T>(string url, object content, NameValueCollection @params = null)
        {
            try
            {
                var uri = @params.BuildUrl(url);
                var stringContent =
                    new StringContent(_jsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(uri, stringContent).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                return _jsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                throw;
            }
        }

        protected virtual async Task<T> GetAsync<T>(string url, NameValueCollection @params = null)
        {
            try
            {
                var uri = @params.BuildUrl(url);
                return _jsonSerializer.Deserialize<T>(await _httpClient.GetStringAsync(uri).ConfigureAwait(false));
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}