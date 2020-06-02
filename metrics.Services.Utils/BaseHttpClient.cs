using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using metrics.Services.Abstractions;
using metrics.Services.Utils.Helpers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace metrics.Services.Utils
{
    public class BaseHttpClient : IBaseHttpClient
    {
        private readonly HttpClient _httpClient;
        protected readonly ILogger<BaseHttpClient> Logger;

        public BaseHttpClient(
            IHttpClientFactory httpClientFactory,
            ILogger<BaseHttpClient> logger
        )
        {
            Logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        protected virtual async Task<T> PostAsync<T>(string url, object content, NameValueCollection @params = null)
        {
            try
            {
                var uri = @params.BuildUrl(url);
                var stringContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(uri, stringContent);

                response.EnsureSuccessStatusCode();

                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
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
                return JsonConvert.DeserializeObject<T>(await _httpClient.GetStringAsync(uri));
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}