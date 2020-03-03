using System;
using System.Collections.Specialized;
using System.Net.Http;
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
                var httpMessage = new HttpRequestMessage(HttpMethod.Post, uri);
                var stringContent = new StringContent(JsonConvert.SerializeObject(content));
                stringContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                httpMessage.Content = stringContent;
                var response = await _httpClient.SendAsync(httpMessage);

                response.EnsureSuccessStatusCode();

                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
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
                var responseContent = await response.Content.ReadAsStringAsync();
                if (responseContent.Length < 1024)
                {
                    Logger.LogInformation(responseContent);
                }

                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                return default;
            }
        }
    }
}