using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using metrics.Services.Abstract;
using metrics.Services.Helpers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace metrics.Services.Concrete
{
    public class BaseHttpClient : IBaseHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BaseHttpClient> _logger;
        public BaseHttpClient(IHttpClientFactory httpClientFactory, ILogger<BaseHttpClient> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        protected virtual async Task<T> PostAsync<T>(string url, object content, NameValueCollection @params = null)
        {
            try
            {
                var uri = @params.BuildUrl(url);
                var httpMessage = new HttpRequestMessage(HttpMethod.Post, uri);
                var stringContent = new StringContent(JsonConvert.SerializeObject(content));
                stringContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                httpMessage.Content = stringContent;
                var response = await _httpClient.SendAsync(httpMessage);

                response.EnsureSuccessStatusCode();

                return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return default;
            }
        }

        protected virtual async Task<T> GetAsync<T>(string url, NameValueCollection @params = null)
        {
            try
            {
                var uri = @params.BuildUrl(url);
                var response = await _httpClient.GetAsync(uri);

                response.EnsureSuccessStatusCode();

                return JsonConvert.DeserializeObject<T>(
                    response.Content.ReadAsStringAsync().Result
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return default;
            }
        }
    }
}
